#region

using LoESoft.GameServer.networking.incoming;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class MarketHandler : MessageHandlers<MARKET_COMMAND>
    {
        public override MessageID ID => MessageID.MARKET_COMMAND;

        protected override void HandleMessage(Client client, MARKET_COMMAND message) => NotImplementedMessageHandler();
    }
}
