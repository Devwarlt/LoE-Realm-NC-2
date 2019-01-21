#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class InvSwapHandler : MessageHandlers<INVSWAP>
    {
        public override MessageID ID => MessageID.INVSWAP;

        protected override void HandleMessage(Client client, INVSWAP message)
        {
            if (client.Player.Owner == null)
                return;

            if (client.InvSwapEntryMS == -1)
                client.InvSwapEntryMS = GameServer.Manager.Logic.GameTime.TotalElapsedMs;
            else
            {
                if (GameServer.Manager.Logic.GameTime.TotalElapsedMs - client.InvSwapEntryMS > 500)
                    client.CanInvSwap = true;
                else
                    client.CanInvSwap = false;
            }

            if (!client.CanInvSwap)
            {
                client.Player.SendHelp("You cannot swap items that fast.");
                return;
            }

            var en1 = client.Player.Owner.GetEntity(message.SlotObject1.ObjectId);
            var en2 = client.Player.Owner.GetEntity(message.SlotObject2.ObjectId);
            var con1 = en1 as IContainer;
            var con2 = en2 as IContainer;

            if (message.SlotObject1.SlotId == 254 || message.SlotObject1.SlotId == 255 ||
                message.SlotObject2.SlotId == 254 || message.SlotObject2.SlotId == 255)
            {
                if (message.SlotObject2.SlotId == 254)
                    if (client.Player.HealthPotions < 6)
                    {
                        client.Player.HealthPotions++;
                        con1.Inventory[message.SlotObject1.SlotId] = null;
                    }
                if (message.SlotObject2.SlotId == 255)
                    if (client.Player.MagicPotions < 6)
                    {
                        client.Player.MagicPotions++;
                        con1.Inventory[message.SlotObject1.SlotId] = null;
                    }
                if (message.SlotObject1.SlotId == 254)
                    if (client.Player.HealthPotions > 0)
                    {
                        client.Player.HealthPotions--;
                        con2.Inventory[message.SlotObject2.SlotId] = null;
                    }
                if (message.SlotObject1.SlotId == 255)
                    if (client.Player.MagicPotions > 0)
                    {
                        client.Player.MagicPotions--;
                        con2.Inventory[message.SlotObject1.SlotId] = null;
                    }
                if (en1 is Player)
                    (en1 as Player).Client.SendMessage(new INVRESULT { Result = 0 });
                else if (en2 is Player)
                    (en2 as Player).Client.SendMessage(new INVRESULT { Result = 0 });
                return;
            }
            //TODO: locker
            Item item1 = con1.Inventory[message.SlotObject1.SlotId];
            Item item2 = con2.Inventory[message.SlotObject2.SlotId];
            List<ushort> publicbags = new List<ushort>
                {
                    0x0500,
                    0x0506,
                    0x0501
                };
            if (en1.Dist(en2) > 1)
            {
                if (en1 is Player)
                    (en1 as Player).Client.SendMessage(new INVRESULT
                    {
                        Result = -1
                    });
                else if (en2 is Player)
                    (en2 as Player).Client.SendMessage(new INVRESULT
                    {
                        Result = -1
                    });
                en1.UpdateCount++;
                en2.UpdateCount++;
                return;
            }

            if (item1 != null && item2 != null && item1.Quantity > 0 && item2.Quantity > 0 && en1 is Player && en2 is Player && en1 == en2 && message.SlotObject1.SlotId != message.SlotObject2.SlotId)
            {
                int quantity = item1.Quantity;

                for (int i = 1; i <= quantity; i++)
                {
                    string name2 = Regex.Replace(item2.ObjectId, "\\d+", "") + (item2.Quantity + 1);

                    if (GameServer.Manager.GameData.IdToObjectType.TryGetValue(name2, out ushort objType))
                    {
                        string name1 = Regex.Replace(item1.ObjectId, "\\d+", "") + (item1.Quantity - 1);

                        item2 = GameServer.Manager.GameData.Items[GameServer.Manager.GameData.IdToObjectType[name2]];
                        item1 = GameServer.Manager.GameData.IdToObjectType.TryGetValue(name1, out objType) == true ? GameServer.Manager.GameData.Items[GameServer.Manager.GameData.IdToObjectType[name1]] : null;

                        con1.Inventory[message.SlotObject1.SlotId] = item1;
                        con2.Inventory[message.SlotObject2.SlotId] = item2;

                        (en1 as Player).CalculateBoost();
                        client.Player.SaveToCharacter();
                        client.Save();
                        en1.UpdateCount++;
                    }
                    else
                        break;
                }
                return;
            }

            if (con2 is OneWayContainer)
            {
                con1.Inventory[message.SlotObject1.SlotId] = null;
                con2.Inventory[message.SlotObject2.SlotId] = null;
                client.Player.DropBag(item1);
                en1.UpdateCount++;
                en2.UpdateCount++;
                return;
            }
            if (con1 is OneWayContainer)
            {
                if (con2.Inventory[message.SlotObject2.SlotId] != null)
                {
                    (en2 as Player)?.Client.SendMessage(new INVRESULT { Result = -1 });
                    (con1 as OneWayContainer).UpdateCount++;
                    en2.UpdateCount++;
                    return;
                }

                List<int> giftsList = client.Account.Gifts.ToList();
                giftsList.Remove(con1.Inventory[message.SlotObject1.SlotId].ObjectType);
                int[] result = giftsList.ToArray();
                client.Account.Gifts = result;
                client.Account.FlushAsync();

                con1.Inventory[message.SlotObject1.SlotId] = null;
                con2.Inventory[message.SlotObject2.SlotId] = item1;
                (en2 as Player).CalculateBoost();
                client.Player.SaveToCharacter();
                en1.UpdateCount++;
                en2.UpdateCount++;
                (en2 as Player).Client.SendMessage(new INVRESULT { Result = 0 });
                return;
            }

            if (en1 is Player && en2 is Player & en1.Id != en2.Id)
                return;

            con1.Inventory[message.SlotObject1.SlotId] = item2;
            con2.Inventory[message.SlotObject2.SlotId] = item1;

            if (item2 != null)
            {
                if (publicbags.Contains(en1.ObjectType) && item2.Soulbound)
                {
                    client.Player.DropBag(item2);
                    con1.Inventory[message.SlotObject1.SlotId] = null;
                }
            }
            if (item1 != null)
            {
                if (publicbags.Contains(en2.ObjectType) && item1.Soulbound)
                {
                    client.Player.DropBag(item1);
                    con2.Inventory[message.SlotObject2.SlotId] = null;
                }
            }
            en1.UpdateCount++;
            en2.UpdateCount++;

            if (en1 is Player)
            {
                if (en1.Owner.Name == "Vault")
                    (en1 as Player).Client.Player.SaveToCharacter();
                (en1 as Player).CalculateBoost();
                (en1 as Player).Client.SendMessage(new INVRESULT { Result = 0 });
            }
            if (en2 is Player)
            {
                if (en2.Owner.Name == "Vault")
                    (en2 as Player).Client.Player.SaveToCharacter();
                (en2 as Player).CalculateBoost();
                (en2 as Player).Client.SendMessage(new INVRESULT { Result = 0 });
            }

            if (client.Player.Owner is Vault)
                if ((client.Player.Owner as Vault).PlayerOwnerName == client.Account.Name)
                    return;

            client.Player.SaveToCharacter();
            client.CanInvSwap = false;
        }

        private bool IsValid(Item item1, Item item2, IContainer con1, IContainer con2, INVSWAP packet, Client client)
        {
            if (con2 is Container || con2 is OneWayContainer)
                return true;

            bool ret = false;

            if (con1 is OneWayContainer || con1 is Container)
            {
                ret = con2.AuditItem(item1, packet.SlotObject2.SlotId);

                if (!ret)
                {
                    log4net.FatalFormat("Cheat engine detected for player {0},\nInvalid InvSwap. {1} instead of {2}",
                            client.Player.Name, GameServer.Manager.GameData.Items[packet.SlotObject1.ObjectType].ObjectId, item1.ObjectId);

                    foreach (Player player in client.Player.Owner.Players.Values)
                        if (player.Client.Account.AccountType >= (int)LoESoft.Core.config.AccountType.DEVELOPER)
                            player.SendInfo(string.Format("Cheat engine detected for player {0},\nInvalid InvSwap. {1} instead of {2}",
                                client.Player.Name, GameServer.Manager.GameData.Items[packet.SlotObject1.ObjectType].ObjectId, item1.ObjectId));
                }
            }
            if (con1 is Player && con2 is Player)
            {
                ret = con1.AuditItem(item1, packet.SlotObject2.SlotId) && con2.AuditItem(item2, packet.SlotObject1.SlotId);

                if (!ret)
                {
                    log4net.FatalFormat("Cheat engine detected for player {0},\nInvalid InvSwap. {1} instead of {2}",
                            client.Player.Name, item1.ObjectId, GameServer.Manager.GameData.Items[packet.SlotObject2.ObjectType].ObjectId);

                    foreach (Player player in client.Player.Owner.Players.Values)
                        if (player.Client.Account.AccountType >= (int)LoESoft.Core.config.AccountType.DEVELOPER)
                            player.SendInfo(string.Format("Cheat engine detected for player {0},\nInvalid InvSwap. {1} instead of {2}",
                                client.Player.Name, item1.ObjectId, GameServer.Manager.GameData.Items[packet.SlotObject2.ObjectType].ObjectId));
                }
            }

            return ret;
        }
    }
}