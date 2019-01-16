#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class BuyHandler : MessageHandlers<BUY>
    {
        public override MessageID ID => MessageID.BUY;

        protected override void HandleMessage(Client client, BUY message) => Handle(client.Player, message.ObjectId, message.Quantity);

        private void Handle(Player player, int objectId, int quantity)
        {
            if (player.Owner == null)
                return;
            if (player.Owner.GetEntity(objectId) is SellableObject obj)
                for (int i = 0; i < quantity; i++)
                    obj.Buy(player);
        }
    }
}