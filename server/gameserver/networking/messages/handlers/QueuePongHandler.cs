#region

using LoESoft.GameServer.networking.incoming;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class QueuePongHandler : MessageHandlers<QUEUE_PONG>
    {
        public override MessageID ID => MessageID.QUEUE_PONG;

        protected override void HandleMessage(Client client, QUEUE_PONG message) => NotImplementedMessageHandler();
    }
}