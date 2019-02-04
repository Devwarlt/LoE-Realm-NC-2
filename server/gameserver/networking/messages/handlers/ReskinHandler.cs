#region

using LoESoft.GameServer.networking.incoming;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class ReskinHandler : MessageHandlers<RESKIN>
    {
        public override MessageID ID => MessageID.RESKIN;

        protected override void HandleMessage(Client client, RESKIN message)
        {
            if (client.Player.Owner == null)
                return;

            if (message.SkinId == 0)
                client.Player.PlayerSkin = 0;
            else
                client.Player.PlayerSkin = message.SkinId;
            client.Player.UpdateCount++;
            client.Player.SaveToCharacter();
        }
    }
}