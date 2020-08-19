#region

using CA.Extensions.Concurrent;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Linq;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class GuildRemovePacketHandler : MessageHandlers<GUILDREMOVE>
    {
        public override MessageID ID => MessageID.GUILDREMOVE;

        protected override void HandleMessage(Client client, GUILDREMOVE message) => Handle(client, message.Name);

        private void Handle(Client client, string name)
        {
            if (client?.Player == null)
                return;

            var player = client.Player;

            if (client.Account.Name.Equals(name))
            {
                GameServer.Manager.Chat.Guild(player, $"{player.Name} has left the guild.", true);

                if (!GameServer.Manager.Database.RemoveFromGuild(client.Account))
                {
                    player.SendError("Guild not found.");
                    return;
                }

                player.Guild = "";
                player.GuildRank = -1;

                client.Account.FlushAsync();

                return;
            }

            var targetAccId = Convert.ToInt32(GameServer.Manager.Database.ResolveId(name));

            if (targetAccId == 0)
            {
                player.SendError("Player not found");
                return;
            }

            var targetClient = GameServer.Manager.GetManager.Clients.ValueWhereAsParallel(_ => _.Account != null && _.AccountId.Equals(targetAccId.ToString())).FirstOrDefault();
            if (targetClient != null)
            {
                if (client.Account.GuildRank >= 20 && client.Account.GuildId == targetClient.Account.GuildId && client.Account.GuildRank > targetClient.Account.GuildRank)
                {
                    Player targetPlayer = targetClient.Player;

                    if (!GameServer.Manager.Database.RemoveFromGuild(targetClient.Account))
                    {
                        player.SendError("Guild not found.");
                        return;
                    }

                    targetPlayer.Guild = "";
                    targetPlayer.GuildRank = -1;

                    client.Account.FlushAsync();

                    GameServer.Manager.Chat.Guild(player, $"{targetPlayer.Name} has been kicked from the guild by {player.Name}.", true);
                    targetPlayer.SendInfo("You have been kicked from the guild.");
                    return;
                }
                else
                {
                    player.SendError("Can't remove member. Insufficient privileges.");
                    return;
                }
            }

            var targetAccount = GameServer.Manager.Database.GetAccountById(targetAccId.ToString());

            if (client.Account.GuildRank >= 20 && client.Account.GuildId == targetAccount.GuildId && client.Account.GuildRank > targetAccount.GuildRank)
            {
                if (!GameServer.Manager.Database.RemoveFromGuild(targetAccount))
                {
                    player.SendError("Guild not found.");
                    return;
                }

                GameServer.Manager.Chat.Guild(player, $"{targetAccount.Name} has been kicked from the guild by {player.Name}.", true);
                player.SendInfo("You have been kicked from the guild.");
                return;
            }

            player.SendError("Can't remove member. Insufficient privileges.");
        }
    }
}
