#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class LeaveArenaHandler : MessageHandlers<ACCEPT_ARENA_DEATH>
    {
        public override MessageID ID => MessageID.ACCEPT_ARENA_DEATH;

        protected override void HandleMessage(Client client, ACCEPT_ARENA_DEATH message)
        {
            if (client.Player.Owner == null)
                return;

            var world = GameServer.Manager.GetWorld(client.Player.Owner.Id);

            if (world.Id == (int)WorldID.NEXUS_ID)
            {
                client.SendMessage(new TEXT
                {
                    Stars = -1,
                    BubbleTime = 0,
                    Name = "",
                    Text = "server.already_nexus",
                    NameColor = 0x123456,
                    TextColor = 0x123456
                });
                return;
            }
            client.Reconnect(new RECONNECT
            {
                Host = "",
                Port = Settings.GAMESERVER.GAME_PORT,
                GameId = (int)WorldID.NEXUS_ID,
                Name = "nexus.Nexus",
                Key = Empty<byte>.Array,
            });
        }
    }
}