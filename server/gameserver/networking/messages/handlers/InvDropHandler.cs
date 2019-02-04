#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class InvDropHandler : MessageHandlers<INVDROP>
    {
        private readonly Random invRand = new Random();

        public override MessageID ID => MessageID.INVDROP;

        protected override void HandleMessage(Client client, INVDROP message)
        {
            if (client.Player.Owner == null)
                return;

            if (message.SlotObject.ObjectId != client.Player.Id)
                return;

            if (client.InvDropEntryMS == -1)
                client.InvDropEntryMS = GameServer.Manager.Logic.GameTime.TotalElapsedMs;
            else
            {
                if (GameServer.Manager.Logic.GameTime.TotalElapsedMs - client.InvDropEntryMS > 500)
                    client.CanInvDrop = true;
                else
                    client.CanInvDrop = false;
            }

            if (!client.CanInvDrop)
            {
                client.Player.SendHelp("You cannot drop items that fast.");
                return;
            }

            if (TradeManager.TradingPlayers.Contains(client.Player))
                return;

            //TODO: locker again
            const ushort NORM_BAG = 0x0500;
            const ushort SOUL_BAG = 0x0507;

            Entity entity = client.Player.Owner.GetEntity(message.SlotObject.ObjectId);
            IContainer con = entity as IContainer;
            Item item = null;
            if (message.SlotObject.SlotId == 254)
            {
                client.Player.HealthPotions--;
                item = GameServer.Manager.GameData.Items[0xa22];
            }
            else if (message.SlotObject.SlotId == 255)
            {
                client.Player.MagicPotions--;
                item = GameServer.Manager.GameData.Items[0xa23];
            }
            else
            {
                if (con.Inventory[message.SlotObject.SlotId] == null)
                    return;

                item = con.Inventory[message.SlotObject.SlotId];
                con.Inventory[message.SlotObject.SlotId] = null;
            }
            entity.UpdateCount++;

            if (item != null)
            {
                Container container;
                if (item.Soulbound)
                {
                    container = new Container(SOUL_BAG, 1000 * 30, true)
                    {
                        BagOwners = new string[1] { client.Player.AccountId }
                    };
                }
                else
                {
                    container = new Container(NORM_BAG, 1000 * 30, true);
                }
                float bagx = entity.X + (float)((invRand.NextDouble() * 2 - 1) * 0.5);
                float bagy = entity.Y + (float)((invRand.NextDouble() * 2 - 1) * 0.5);
                try
                {
                    container.Inventory[0] = item;
                    container.Move(bagx, bagy);
                    container.Size = 75;
                    client.Player.Owner.EnterWorld(container);

                    if (entity is Player)
                    {
                        (entity as Player).Client.SendMessage(new INVRESULT
                        {
                            Result = 0
                        });
                        (entity as Player).Client.Player.SaveToCharacter();
                    }

                    if (client.Player.Owner is Vault)
                        if ((client.Player.Owner as Vault).PlayerOwnerName == client.Account.Name)
                            return;
                }
                catch (Exception ex)
                {
                    log4net.Error(ex);
                    log4net.InfoFormat(client.Player.Name + " just attempted to dupe.");
                }
            }

            client.CanInvDrop = false;
        }
    }
}