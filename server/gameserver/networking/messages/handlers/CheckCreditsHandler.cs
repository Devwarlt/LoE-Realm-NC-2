#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class CheckCreditsHandler : MessageHandlers<CHECKCREDITS>
    {
        public override MessageID ID => MessageID.CHECKCREDITS;

        protected override void HandleMessage(Client client, CHECKCREDITS message) => Handle(client);

        private void Handle(Client client)
        {
            client.Account.FlushAsync();
            client.Account.Reload();
            Manager.Logic.AddPendingAction(t =>
            {
                client.Player.Credits = client.Player.Client.Account.Credits;
                client.Player.UpdateCount++;
            }, PendingPriority.Networking);
        }
    }
}