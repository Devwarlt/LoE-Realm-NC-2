#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System.Collections.Generic;
using System.Linq;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class UseItemHandler : MessageHandlers<USEITEM>
    {
        public override MessageID ID => MessageID.USEITEM;

        protected override void HandleMessage(Client client, USEITEM message)
        {
            if (client.Player.Owner == null)
                return;
            try
            {
                if (!(client.Player.Owner.GetEntity(message.SlotObject.ObjectId) is IContainer container))
                    return;

                if (TradeManager.TradingPlayers.Contains(client.Player))
                    return;

                Item item;

                switch (message.SlotObject.SlotId)
                {
                    case 254:
                        item = GameServer.Manager.GameData.Items[message.SlotObject.ObjectType];

                        if (item.ObjectId != "Health Potion")
                        {
                            log4net.FatalFormat("Cheat engine detected for player {0},\nItem should be a Health Potion, but its {1}.",
                                client.Player.Name, item.ObjectId);
                            foreach (var player in client.Player.Owner.GetPlayers())
                                if (player.Client.Account.AccountType >= (int)Core.config.AccountType.DEVELOPER)
                                    player.SendInfo(string.Format("Cheat engine detected for player {0},\nItem should be a Health Potion, but its {1}.",
                                        client.Player.Name, item.ObjectId));
                            GameServer.Manager.TryDisconnect(client, DisconnectReason.HP_POTION_CHEAT_ENGINE);
                            return;
                        }

                        if (client.Player.HealthPotions > 0)
                            client.Player.HealthPotions--;
                        else
                        {
                            if (client.Account.Credits > client.Player.HpPotionPrice)
                            {
                                switch (client.Player.HpPotionPrice)
                                {
                                    case 5:
                                        {
                                            if (client.Player.HpFirstPurchaseTime)
                                            {
                                                client.Player.HpPotionPrice = 5;
                                                client.Player.HpFirstPurchaseTime = false;
                                            }
                                            client.Player.HpPotionPrice = 10;
                                        }
                                        break;

                                    case 10:
                                        { client.Player.HpPotionPrice = 20; }
                                        break;

                                    case 20:
                                        { client.Player.HpPotionPrice = 40; }
                                        break;

                                    case 40:
                                        { client.Player.HpPotionPrice = 80; }
                                        break;

                                    case 80:
                                        { client.Player.HpPotionPrice = 120; }
                                        break;

                                    case 120:
                                        { client.Player.HpPotionPrice = 200; }
                                        break;

                                    case 200:
                                        { client.Player.HpPotionPrice = 300; }
                                        break;

                                    case 300:
                                        { client.Player.HpPotionPrice = 450; }
                                        break;

                                    case 450:
                                        { client.Player.HpPotionPrice = 600; }
                                        break;

                                    case 600:
                                        break;
                                }
                                client.Player.Owner.Timers.Add(new WorldTimer(8000, (world, j) =>
                                {
                                    if (client == null)
                                        return;

                                    if (client.Player == null)
                                        return;

                                    switch (client?.Player?.HpPotionPrice)
                                    {
                                        case 5:
                                            break;

                                        case 10:
                                            {
                                                client.Player.HpFirstPurchaseTime = true;
                                                client.Player.HpPotionPrice = 5;
                                            }
                                            break;

                                        case 20:
                                            { client.Player.HpPotionPrice = 10; }
                                            break;

                                        case 40:
                                            { client.Player.HpPotionPrice = 20; }
                                            break;

                                        case 80:
                                            { client.Player.HpPotionPrice = 40; }
                                            break;

                                        case 120:
                                            { client.Player.HpPotionPrice = 80; }
                                            break;

                                        case 200:
                                            { client.Player.HpPotionPrice = 120; }
                                            break;

                                        case 300:
                                            { client.Player.HpPotionPrice = 200; }
                                            break;

                                        case 450:
                                            { client.Player.HpPotionPrice = 300; }
                                            break;

                                        case 600:
                                            { client.Player.HpPotionPrice = 450; }
                                            break;
                                    }
                                }));

                                int currentCredits = client.Player.Credits - client.Player.HpPotionPrice;

                                GameServer.Manager.Database.UpdateCredit(client.Account, -client.Player.HpPotionPrice);

                                client.Player.Credits = client.Account.Credits = currentCredits;
                                client.Character.HP += 100;
                                client.Player.SaveToCharacter();
                            }
                        }
                        break;

                    case 255:
                        item = GameServer.Manager.GameData.Items[message.SlotObject.ObjectType];

                        if (item.ObjectId != "Magic Potion")
                        {
                            log4net.FatalFormat("Cheat engine detected for player {0},\nItem should be a Magic Potion, but its {1}.",
                                client.Player.Name, item.ObjectId);
                            foreach (var player in client.Player.Owner.GetPlayers().Where(player => player.Client.Account.AccountType >= (int)Core.config.AccountType.DEVELOPER))
                                player.SendInfo($"Cheat engine detected for player {client.Player.Name},\nItem should be a Magic Potion, but its {item.ObjectId}.");
                            GameServer.Manager.TryDisconnect(client, DisconnectReason.MP_POTION_CHEAT_ENGINE);
                            return;
                        }

                        if (client.Player.MagicPotions > 0)
                            client.Player.MagicPotions--;
                        else
                        {
                            if (client.Account.Credits > client.Player.MpPotionPrice)
                            {
                                switch (client.Player.MpPotionPrice)
                                {
                                    case 5:
                                        {
                                            if (client.Player.MpFirstPurchaseTime)
                                            {
                                                client.Player.MpPotionPrice = 5;
                                                client.Player.MpFirstPurchaseTime = false;
                                            }
                                            client.Player.MpPotionPrice = 10;
                                        }
                                        break;

                                    case 10:
                                        { client.Player.MpPotionPrice = 20; }
                                        break;

                                    case 20:
                                        { client.Player.MpPotionPrice = 40; }
                                        break;

                                    case 40:
                                        { client.Player.MpPotionPrice = 80; }
                                        break;

                                    case 80:
                                        { client.Player.MpPotionPrice = 120; }
                                        break;

                                    case 120:
                                        { client.Player.MpPotionPrice = 200; }
                                        break;

                                    case 200:
                                        { client.Player.MpPotionPrice = 300; }
                                        break;

                                    case 300:
                                        { client.Player.MpPotionPrice = 450; }
                                        break;

                                    case 450:
                                        { client.Player.MpPotionPrice = 600; }
                                        break;

                                    case 600:
                                        break;
                                }

                                client?.Player?.Owner?.Timers.Add(new WorldTimer(8000, (world, j) =>
                                {
                                    if (client == null)
                                        return;

                                    if (client.Player == null)
                                        return;

                                    switch (client.Player.MpPotionPrice)
                                    {
                                        case 5:
                                            break;

                                        case 10:
                                            {
                                                client.Player.MpFirstPurchaseTime = true;
                                                client.Player.MpPotionPrice = 5;
                                            }
                                            break;

                                        case 20:
                                            { client.Player.MpPotionPrice = 10; }
                                            break;

                                        case 40:
                                            { client.Player.MpPotionPrice = 20; }
                                            break;

                                        case 80:
                                            { client.Player.MpPotionPrice = 40; }
                                            break;

                                        case 120:
                                            { client.Player.MpPotionPrice = 80; }
                                            break;

                                        case 200:
                                            { client.Player.MpPotionPrice = 120; }
                                            break;

                                        case 300:
                                            { client.Player.MpPotionPrice = 200; }
                                            break;

                                        case 450:
                                            { client.Player.MpPotionPrice = 300; }
                                            break;

                                        case 600:
                                            { client.Player.MpPotionPrice = 450; }
                                            break;
                                    }
                                }));

                                int currentCredits = client.Player.Credits - client.Player.MpPotionPrice;

                                GameServer.Manager.Database.UpdateCredit(client.Account, -client.Player.MpPotionPrice);

                                client.Player.Credits = client.Account.Credits = currentCredits;
                                client.Character.MP += 100;
                                client.Player.SaveToCharacter();
                            }
                        }
                        break;

                    default:
                        item = container.Inventory[message.SlotObject.SlotId];
                        break;
                }
                if (item != null)
                {
                    if (!client.Player.Activate(GameServer.Manager.Logic.GameTime, item, message))
                    {
                        if (item.Consumable)
                        {
                            if (item.SuccessorId != null)
                            {
                                if (item.SuccessorId != item.ObjectId)
                                {
                                    if (message.SlotObject.SlotId != 254 && message.SlotObject.SlotId != 255)
                                    {
                                        container.Inventory[message.SlotObject.SlotId] =
                                            GameServer.Manager.GameData.Items[
                                                GameServer.Manager.GameData.IdToObjectType[item.SuccessorId]];
                                        client.Player.Owner.GetEntity(message.SlotObject.ObjectId).UpdateCount++;
                                    }
                                }
                            }
                            else
                            {
                                if (message.SlotObject.SlotId != 254 && message.SlotObject.SlotId != 255)
                                {
                                    container.Inventory[message.SlotObject.SlotId] = null;
                                    client.Player.Owner.GetEntity(message.SlotObject.ObjectId).UpdateCount++;
                                }
                            }

                            if (container is OneWayContainer)
                            {
                                List<int> giftsList = client.Account.Gifts.ToList();
                                giftsList.Remove(message.SlotObject.ObjectType);
                                int[] result = giftsList.ToArray();
                                client.Account.Gifts = result;
                                client.Account.FlushAsync();
                            }
                        }
                    }
                }
                if (message.SlotObject.SlotId != 254 && message.SlotObject.SlotId != 255)
                    if (container.SlotTypes[message.SlotObject.SlotId] != -1)
                        client.Player.FameCounter.UseAbility();

                ((Entity)container).UpdateCount++;
                client.Player.UpdateCount++;
                client.Player.SaveToCharacter();
            }
            catch { return; }
        }
    }
}
