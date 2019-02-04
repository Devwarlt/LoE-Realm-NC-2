using LoESoft.Core.models;
using LoESoft.GameServer.realm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static LoESoft.GameServer.networking.Client;

namespace LoESoft.GameServer.networking
{
    public class ClientManager
    {
        public ConcurrentDictionary<string, Client> Clients { get; private set; } = new ConcurrentDictionary<string, Client>();

        public bool AddClient(Client client)
        {
            if (Clients.ContainsKey(client.Account.AccountId))
                TryDisconnect(client, DisconnectReason.OLD_CLIENT_DISCONNECT);

            return Clients.TryAdd(client.Account.AccountId, client);
        }

        public IEnumerable<string> GetPlayersName()
        {
            foreach (var client in Clients.Values)
                yield return client?.Account.Name;
        }

        public void RemoveClient(Client client, DisconnectReason reason)
        {
            try
            {
                if (Clients.ContainsKey(client.Account.AccountId))
                {
                    Clients.TryRemove(client.Account.AccountId, out Client oldclient);

                    Log.Info($"[({(int)reason}) {reason.ToString()}] Disconnect player '{oldclient.Account.Name} (Account ID: {oldclient.Account.AccountId})'.");

                    using (oldclient)
                    {
                        oldclient.Save();

                        var player = oldclient.Player;

                        if (player != null)
                            player.Owner.LeaveWorld(player);

                        oldclient.State = ProtocolState.Disconnected;
                        oldclient.Socket.Close();
                    }
                }
                else
                {
                    Log.Info($"[({(int)reason}) {reason.ToString()}] Disconnect player '{client.Account.Name} (Account ID: {client.Account.AccountId})'.");

                    using (client)
                    {
                        client.Save();

                        var player = client.Player;

                        if (player != null)
                            player.Owner.LeaveWorld(player);

                        client.State = ProtocolState.Disconnected;
                    }
                }
            }
            catch { }
        }

        public ConnectionProtocol TryConnect(Client client)
        {
            if (AccessDenied)
                return new ConnectionProtocol(false, ErrorIDs.ACCESS_DENIED_DUE_RESTART); // Prevent account in use issue along restart.
            else
            {
                try
                {
                    if (client.Account.Banned)
                        return new ConnectionProtocol(false, ErrorIDs.ACCOUNT_BANNED);

                    if (Clients.Keys.Count >= GameServer.Manager.MaxClients) // When server is full.
                        return new ConnectionProtocol(false, ErrorIDs.SERVER_FULL);

                    return new ConnectionProtocol(AddClient(client), ErrorIDs.NORMAL_CONNECTION); // Normal connection with reconnect type.
                }
                catch (Exception e) { GameServer.log.Error(e); }

                return new ConnectionProtocol(false, ErrorIDs.LOST_CONNECTION); // User dropped connection while reconnect.
            }
        }

        public void TryDisconnect(Client client, DisconnectReason reason = DisconnectReason.UNKNOW_ERROR_INSTANCE)
        {
            if (client == null)
                return;

            RemoveClient(client, reason == DisconnectReason.UNKNOW_ERROR_INSTANCE ? DisconnectReason.REALM_MANAGER_DISCONNECT : reason);
        }
    }
}