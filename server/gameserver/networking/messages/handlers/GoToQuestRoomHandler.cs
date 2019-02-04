#region

using LoESoft.Core.config;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class GoToQuestRoomHandler : MessageHandlers<QUEST_ROOM_MSG>
    {
        public override MessageID ID => MessageID.QUEST_ROOM_MSG;

        protected override void HandleMessage(Client client, QUEST_ROOM_MSG message)
        {
            var world = GameServer.Manager.GetWorld((int)WorldID.DAILY_QUEST_ID);

            client.Reconnect(new RECONNECT
            {
                Host = "",
                Port = Settings.GAMESERVER.GAME_PORT,
                GameId = world.Id,
                Name = world.Name,
                Key = world.PortalKey,
            });
        }
    }
}