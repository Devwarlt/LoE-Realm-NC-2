#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.GameServer.networking.error;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static LoESoft.GameServer.networking.Client;
using FAILURE = LoESoft.GameServer.networking.outgoing.FAILURE;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class HelloHandler : MessageHandlers<HELLO>
    {
        public override MessageID ID => MessageID.HELLO;

        protected override void HandleMessage(Client client, HELLO message)
        {
            var versionStatus = Settings.CheckClientVersion(message.BuildVersion);

            if (!versionStatus.Value)
            {
                client.SendMessage(new FAILURE
                {
                    ErrorId = (int)FailureIDs.JSON_DIALOG,
                    ErrorDescription =
                        JSONErrorIDHandler.
                            FormatedJSONError(
                                errorID: ErrorIDs.OUTDATED_CLIENT,
                                labels: new[] { "{CLIENT_BUILD_VERSION}", "{SERVER_BUILD_VERSION}" },
                                arguments: new[] { message.BuildVersion, versionStatus.Key }
                            )
                });

                GameServer.Manager.TryDisconnect(client, DisconnectReason.OUTDATED_CLIENT);

                return;
            }

            var s1 = GameServer.Manager.Database.Verify(message.GUID, message.Password, out DbAccount acc);

            if (s1 == LoginStatus.AccountNotExists)
            {
                var s2 = GameServer.Manager.Database.Register(message.GUID, message.Password, true, out acc); //Register guest but do not allow join game.

                client.SendMessage(new FAILURE()
                {
                    ErrorId = (int)FailureIDs.JSON_DIALOG,
                    ErrorDescription =
                        JSONErrorIDHandler.
                            FormatedJSONError(
                                errorID: ErrorIDs.DISABLE_GUEST_ACCOUNT,
                                labels: null,
                                arguments: null
                            )
                });

                GameServer.Manager.TryDisconnect(client, DisconnectReason.DISABLE_GUEST_ACCOUNT);

                return;
            }
            else if (s1 == LoginStatus.InvalidCredentials)
            {
                client.SendMessage(new FAILURE
                {
                    ErrorId = (int)FailureIDs.DEFAULT,
                    ErrorDescription = "Bad login."
                });

                GameServer.Manager.TryDisconnect(client, DisconnectReason.BAD_LOGIN);
            }

            client.ConnectedBuild = message.BuildVersion;
            client.Account = acc;
            client.AccountId = acc.AccountId;

            if (AccountInUseManager.ContainsKey(client.AccountId))
            {
                do
                {
                    var timeout = client.CheckAccountInUseTimeout;

                    if (timeout <= 0)
                        break;

                    var outgoing = new List<Message>
                        {
                            new FAILURE
                            {
                                ErrorId = (int)ErrorIDs.NORMAL_CONNECTION,
                                ErrorDescription = $"Account in use ({timeout:n0} second{(timeout > 1 ? "s" : "")} until timeout)."
                            },
                            new FAILURE
                            {
                                ErrorId = (int)ErrorIDs.NORMAL_CONNECTION,
                                ErrorDescription = $"Connection failed! Retrying..."
                            }
                        };

                    client.SendMessage(outgoing);

                    Thread.Sleep(3 * 1000);

                    if (client.CheckAccountInUseTimeout <= 0)
                        break;
                } while (client.Socket.Connected && client.State != ProtocolState.Disconnected);

                client.RemoveAccountInUse();
            }

            var TryConnect = GameServer.Manager.TryConnect(client);

            if (!TryConnect.Connected)
            {
                var errorID = TryConnect.ErrorID;
                string[] labels;
                string[] arguments;
                DisconnectReason reason;

                switch (errorID)
                {
                    case ErrorIDs.SERVER_FULL:
                        {
                            labels = new[] { "{MAX_USAGE}" };
                            arguments = new[] { GameServer.Manager.MaxClients.ToString() };
                            reason = DisconnectReason.SERVER_FULL;
                        }
                        break;

                    case ErrorIDs.ACCOUNT_BANNED:
                        {
                            labels = new[] { "{CLIENT_NAME}" };
                            arguments = new[] { acc.Name };
                            reason = DisconnectReason.ACCOUNT_BANNED;
                        }
                        break;

                    case ErrorIDs.INVALID_DISCONNECT_KEY:
                        {
                            labels = new[] { "{CLIENT_NAME}" };
                            arguments = new[] { acc.Name };
                            reason = DisconnectReason.INVALID_DISCONNECT_KEY;
                        }
                        break;

                    case ErrorIDs.LOST_CONNECTION:
                        {
                            labels = new[] { "{CLIENT_NAME}" };
                            arguments = new[] { acc.Name };
                            reason = DisconnectReason.LOST_CONNECTION;
                        }
                        break;

                    case ErrorIDs.ACCESS_DENIED_DUE_RESTART:
                        {
                            labels = new[] { "{CLIENT_NAME}" };
                            arguments = new[] { acc.Name };
                            reason = DisconnectReason.ACCESS_DENIED_DUE_RESTART;
                        }
                        break;

                    default:
                        {
                            labels = new[] { "{UNKNOW_ERROR_INSTANCE}" };
                            arguments = new[] { "connection aborted by unexpected protocol at line <b>340</b> or line <b>346</b> from 'TryConnect' function in RealmManager for security reasons" };
                            reason = DisconnectReason.UNKNOW_ERROR_INSTANCE;
                        }
                        break;
                }

                client.SendMessage(new FAILURE
                {
                    ErrorId = (int)FailureIDs.JSON_DIALOG,
                    ErrorDescription =
                        JSONErrorIDHandler.
                            FormatedJSONError(
                                errorID: errorID,
                                labels: labels,
                                arguments: arguments
                            )
                });

                GameServer.Manager.TryDisconnect(client, reason);

                return;
            }
            else
            {
                var world = GameServer.Manager.GetWorld(message.GameId);

                if (world == null)
                {
                    client.SendMessage(new FAILURE
                    {
                        ErrorId = (int)FailureIDs.DEFAULT,
                        ErrorDescription = "Invalid world."
                    });

                    GameServer.Manager.TryDisconnect(client, DisconnectReason.INVALID_WORLD);

                    return;
                }

                if (world.NeedsPortalKey)
                {
                    if (!world.PortalKey.SequenceEqual(message.Key))
                    {
                        client.SendMessage(new FAILURE
                        {
                            ErrorId = (int)FailureIDs.DEFAULT,
                            ErrorDescription = "Invalid portal key."
                        });

                        GameServer.Manager.TryDisconnect(client, DisconnectReason.INVALID_PORTAL_KEY);

                        return;
                    }

                    if (world.PortalKeyExpired)
                    {
                        client.SendMessage(new FAILURE
                        {
                            ErrorId = (int)FailureIDs.DEFAULT,
                            ErrorDescription = "Portal key expired."
                        });

                        GameServer.Manager.TryDisconnect(client, DisconnectReason.PORTAL_KEY_EXPIRED);

                        return;
                    }
                }

                if (acc.AccountType == (int)AccountType.VIP)
                {
                    var _currentTime = DateTime.UtcNow;
                    var _vipRegistration = acc.AccountLifetime;

                    if (_vipRegistration <= _currentTime)
                    {
                        acc.AccountType = (int)AccountType.REGULAR;
                        acc.FlushAsync();
                        acc.Reload();

                        var _failure = new FAILURE
                        {
                            ErrorId = (int)FailureIDs.JSON_DIALOG,
                            ErrorDescription =
                            JSONErrorIDHandler
                                .FormatedJSONError(
                                    errorID: ErrorIDs.VIP_ACCOUNT_OVER,
                                    labels: new[] { "{CLIENT_NAME}", "{SERVER_TIME}", "{REGISTRATION_TIME}", "{CURRENT_TIME}" },
                                    arguments: new[] { acc.Name, string.Format(new DateProvider(), "{0}", DateTime.UtcNow), string.Format(new DateProvider(), "{0}", acc.AccountLifetime.AddDays(-30)), string.Format(new DateProvider(), "{0}", acc.AccountLifetime) }
                                )
                        };

                        client.SendMessage(_failure);

                        GameServer.Manager.TryDisconnect(client, DisconnectReason.VIP_ACCOUNT_OVER);

                        return;
                    }
                }

                if (message.MapInfo.Length > 0 || world.Id == -6)
                {
                    client.SendMessage(new FAILURE
                    {
                        ErrorId = (int)FailureIDs.DEFAULT,
                        ErrorDescription = "Test map feature isn't working..."
                    });

                    GameServer.Manager.TryDisconnect(client, DisconnectReason.TEST_MAP_NOT_ADDED);
                    return;
                }

                if (!world.IsTickRunning)
                    world.EnableWorldTick();

                if (world.IsLimbo)
                    world = world.GetInstance(client);

                //world.AllowConnection.WaitOne();

                client.Random = new wRandom(world.Seed);
                client.TargetWorld = world.Id;
                client.SendMessage(new MAPINFO
                {
                    Width = world.Map.Width,
                    Height = world.Map.Height,
                    Name = world.Name,
                    Seed = world.Seed,
                    ClientWorldName = world.Name,
                    Difficulty = world.Difficulty,
                    Background = world.Background,
                    AllowTeleport = world.AllowTeleport,
                    ShowDisplays = world.ShowDisplays,
                    ClientXML = world.ClientXml,
                    ExtraXML = GameServer.Manager.GameData.AdditionXml,
                    Music = world.Name
                });
                client.State = ProtocolState.Handshaked;
            }
        }
    }
}