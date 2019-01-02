#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Generic;
using System.Globalization;
using FAILURE = LoESoft.GameServer.networking.outgoing.FAILURE;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class UsePortalHandler : MessageHandlers<USEPORTAL>
    {
        public override MessageID ID => MessageID.USEPORTAL;

        protected override void HandleMessage(Client client, USEPORTAL message) => Manager.Logic.AddPendingAction(t => Handle(client, client.Player, message), PendingPriority.Networking);

        private readonly List<string> Blacklist = new List<string>
        {
            "Cloth Bazaar"
        };

        private void Handle(Client client, Player player, USEPORTAL message)
        {
            if (player?.Owner == null)
                return;

            Portal portal = player.Owner.GetEntity(message.ObjectId) as Portal;

            if (portal == null)
                return;

            if (!portal.Usable)
            {
                player.SendError("{\"key\":\"server.realm_full\"}");
                return;
            }

            World world = portal.WorldInstance;

            if (world == null)
            {
                bool setWorldInstance = true;

                PortalDesc desc = portal.ObjectDesc;

                if (desc == null)
                {
                    client.SendMessage(new FAILURE
                    {
                        ErrorId = (int)FailureIDs.DEFAULT,
                        ErrorDescription = "Portal not found!"
                    });
                }
                else
                {
                    if (Blacklist.Contains(desc.DungeonName))
                    {
                        player.SendHelp($"'{desc.DungeonName}' is under maintenance and disabled to access until further notice from LoESoft Games.");
                        return;
                    }
                    switch (portal.ObjectType)
                    {
                        case 0x0720:
                            world = GameServer.Manager.PlayerVault(client);
                            setWorldInstance = false;
                            break;

                        case 0x0704:
                        case 0x0703: //portal of cowardice
                        case 0x0d40:
                        case 0x070d:
                        case 0x070e:
                            {
                                if (GameServer.Manager.LastWorld.ContainsKey(player.AccountId))
                                {
                                    World w = GameServer.Manager.LastWorld[player.AccountId];

                                    if (w != null && GameServer.Manager.Worlds.ContainsKey(w.Id))
                                        world = w;
                                    else
                                        world = GameServer.Manager.GetWorld((int)WorldID.NEXUS_ID);
                                }
                                else
                                    world = GameServer.Manager.GetWorld((int)WorldID.NEXUS_ID);
                                setWorldInstance = false;
                            }
                            break;

                        case 0x0750:
                            world = GameServer.Manager.GetWorld((int)WorldID.MARKET);
                            break;

                        case 0x071d:
                            world = GameServer.Manager.GetWorld((int)WorldID.NEXUS_ID);
                            break;

                        case 0x0712:
                            world = GameServer.Manager.GetWorld((int)WorldID.NEXUS_ID);
                            break;

                        case 0x1756:
                            world = GameServer.Manager.GetWorld((int)WorldID.DAILY_QUEST_ID);
                            break;

                        case 0x072f:
                            if (player.Guild != null)
                            {
                                //client.Player.SendInfo(
                                //    "Sorry, you are unable to enter the GuildHall because of a possible memory leak, check back later");
                                player.SendInfo("Thanks.");
                            }
                            break;

                        default:
                            {
                                Type worldType =
                                    Type.GetType("LoESoft.GameServer.realm.world." + desc.DungeonName.Replace(" ", string.Empty).Replace("'", string.Empty));
                                if (worldType != null)
                                {
                                    try
                                    {
                                        world = Manager.AddWorld((World)Activator.CreateInstance(worldType,
                                        System.Reflection.BindingFlags.CreateInstance, null, null,
                                        CultureInfo.InvariantCulture, null));
                                    }
                                    catch
                                    {
                                        player.SendError($"Dungeon instance \"{desc.DungeonName}\" isn't declared yet and under maintenance until further notice.");
                                    }
                                }
                                else
                                    player.SendHelp($"Dungeon instance \"{desc.DungeonName}\" isn't declared yet and under maintenance until further notice.");
                            }
                            break;
                    }
                }
                if (setWorldInstance)
                    portal.WorldInstance = world;
            }

            if (world != null)
            {
                if (world.IsFull)
                    player.SendError("{\"key\":\"server.dungeon_full\"}");
                else
                {
                    if (GameServer.Manager.LastWorld.ContainsKey(player.AccountId))
                        GameServer.Manager.LastWorld.TryRemove(player.AccountId, out World dummy);

                    if (player.Owner is Nexus || player.Owner is GameWorld)
                        GameServer.Manager.LastWorld.TryAdd(player.AccountId, player.Owner);

                    client?.Reconnect(new RECONNECT
                    {
                        Host = "",
                        Port = Settings.GAMESERVER.PORT,
                        GameId = world.Id,
                        Name = world.Name,
                        Key = world.PortalKey,
                    });
                }
            }
        }
    }
}