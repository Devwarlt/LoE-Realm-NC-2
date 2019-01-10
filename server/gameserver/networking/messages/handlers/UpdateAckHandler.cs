#region

using LoESoft.GameServer.networking.incoming;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class UpdateAckHandler : MessageHandlers<UPDATEACK>
    {
        public override MessageID ID => MessageID.UPDATEACK;

        protected override void HandleMessage(Client client, UPDATEACK message) => NotImplementedMessageHandler();
    }
}