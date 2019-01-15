#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class PongHandler : MessageHandlers<PONG>
    {
        public override MessageID ID => MessageID.PONG;

        protected override void HandleMessage(Client client, PONG message) => Manager.Logic.AddPendingAction(t => Handle(client, message, t));

        private void Handle(Client client, PONG message, RealmTime t) => client.Player.Pong(t, message);
    }
}