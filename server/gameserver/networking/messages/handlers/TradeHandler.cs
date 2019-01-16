#region

using LoESoft.GameServer.networking.incoming;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class RequestTradeHandler : MessageHandlers<REQUESTTRADE>
    {
        public override MessageID ID => MessageID.REQUESTTRADE;

        protected override void HandleMessage(Client client, REQUESTTRADE message) => client.Player.RequestTrade(Manager.Logic.GameTime, message);
    }

    internal class ChangeTradeHandler : MessageHandlers<CHANGETRADE>
    {
        public override MessageID ID => MessageID.CHANGETRADE;

        protected override void HandleMessage(Client client, CHANGETRADE message) => client.Player.ChangeTrade(Manager.Logic.GameTime, message);
    }

    internal class AcceptTradeHandler : MessageHandlers<ACCEPTTRADE>
    {
        public override MessageID ID => MessageID.ACCEPTTRADE;

        protected override void HandleMessage(Client client, ACCEPTTRADE message) => client.Player.AcceptTrade(Manager.Logic.GameTime, message);
    }

    internal class CancelTradeHandler : MessageHandlers<CANCELTRADE>
    {
        public override MessageID ID => MessageID.CANCELTRADE;

        protected override void HandleMessage(Client client, CANCELTRADE message) => client.Player.CancelTrade(Manager.Logic.GameTime, message);
    }
}