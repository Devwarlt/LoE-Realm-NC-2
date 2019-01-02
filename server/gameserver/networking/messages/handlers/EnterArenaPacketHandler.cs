#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.world;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class EnterArenaPacketHandler : MessageHandlers<ENTER_ARENA>
    {
        public override MessageID ID => MessageID.ENTER_ARENA;

        protected override void HandleMessage(Client client, ENTER_ARENA message) => Handle(client, message);

        private void Handle(Client client, ENTER_ARENA message)
        {
            if (message.Currency == 1)
            {
                if (client.Account.Fame >= 500)
                {
                    Manager.Database.UpdateFame(client.Account, -500);
                }
            }
            else
            {
                if (client.Account.Credits >= 50)
                {
                    Manager.Database.UpdateCredit(client.Account, -50);
                }
            }
            client.Player.UpdateCount++;
            client.Player.SaveToCharacter();

            var world = GameServer.Manager.AddWorld(new Arena());

            client.Reconnect(new RECONNECT
            {
                Host = "",
                Port = Settings.GAMESERVER.PORT,
                GameId = world.Id,
                Name = world.Name,
                Key = Empty<byte>.Array,
            });
        }
    }
}