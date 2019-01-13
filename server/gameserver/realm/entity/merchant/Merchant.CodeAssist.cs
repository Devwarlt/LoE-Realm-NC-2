#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity.merchant
{
    partial class Merchant
    {
        public void Recreate(Merchant x)
        {
            try
            {
                var mrc = new Merchant(x.ObjectType, x.Owner);
                mrc.Move(x.X, x.Y);
                var w = Owner;
                Owner.LeaveWorld(this);
                w.Timers.Add(new WorldTimer(Random.Next(30, 60) * 1000, (world, time) => w.EnterWorld(mrc)));
            }
            catch (Exception) { }
        }

        public override void Buy(Player player)
        {
            if (ObjectType == 0x01ca) //Merchant
            {
                int originalPrice = Price;
                Price = (int) (Price * player.AccountPerks.MerchantDiscount());
                if (TryDeduct(player))
                {
                    for (var i = 4; i < player.Inventory.Length; i++)
                    {
                        try
                        {
                            GameServer.Manager.GameData.ObjectTypeToElement.TryGetValue((ushort) MType, out XElement ist);
                            if (player.Inventory[i] == null &&
                                (player.SlotTypes[i] == 10 ||
                                 player.SlotTypes[i] == Convert.ToInt16(ist.Element("SlotType").Value)))
                            // Exploit fix - No more mnovas as weapons!
                            {
                                player.Inventory[i] = GameServer.Manager.GameData.Items[(ushort) MType];

                                KeyValuePair<string, int> currency = new KeyValuePair<string, int>(null, -1);

                                switch (Currency)
                                {
                                    case CurrencyType.Fame:
                                        {
                                            GameServer.Manager.Database.UpdateFame(player.Client.Account, -Price);
                                            player.CurrentFame = player.Client.Account.Fame;
                                            currency = new KeyValuePair<string, int>("fame", player.CurrentFame);
                                        }
                                        break;

                                    case CurrencyType.Gold:
                                        {
                                            GameServer.Manager.Database.UpdateCredit(player.Client.Account, -Price);
                                            player.Credits = player.Client.Account.Credits;
                                            currency = new KeyValuePair<string, int>("gold", player.Credits);
                                        }
                                        break;

                                    case CurrencyType.FortuneTokens:
                                        {
                                            GameServer.Manager.Database.UpdateTokens(player.Client.Account, -Price);
                                            player.Tokens = player.Client.Account.FortuneTokens;
                                            currency = new KeyValuePair<string, int>("fortune token", player.Tokens);
                                        }
                                        break;

                                    default:
                                        break;
                                }
                                if (1 - player.AccountPerks.MerchantDiscount() > 0 && (currency.Key != null && currency.Value != -1))
                                    player.SendInfo($"You saved {originalPrice - Price} {currency.Key}{(currency.Value > 1 ? "s" : "")} ({(1 - player.AccountPerks.MerchantDiscount()) * 100}% off)!");
                                player.Client.SendMessage(new BUYRESULT
                                {
                                    Result = 0,
                                    Message = "{\"key\":\"server.buy_success\"}"
                                });
                                MRemaining--;
                                player.UpdateCount++;
                                player.SaveToCharacter();
                                UpdateCount++;
                                return;
                            }
                        }
                        catch (Exception) { }
                    }
                    player.Client.SendMessage(new BUYRESULT
                    {
                        Result = 0,
                        Message = "{\"key\":\"server.inventory_full\"}"
                    });
                }
                else
                {
                    if (player.Stars < RankReq)
                    {
                        player.Client.SendMessage(new BUYRESULT
                        {
                            Result = 0,
                            Message = "{\"key\":\"server.not_enough_star\"}"
                        });
                        return;
                    }
                    switch (Currency)
                    {
                        case CurrencyType.Gold:
                            player.Client.SendMessage(new BUYRESULT
                            {
                                Result = BUY_NO_GOLD,
                                Message = "{\"key\":\"server.not_enough_gold\"}"
                            });
                            break;

                        case CurrencyType.Fame:
                            player.Client.SendMessage(new BUYRESULT
                            {
                                Result = BUY_NO_FAME,
                                Message = "{\"key\":\"server.not_enough_fame\"}"
                            });
                            break;

                        case CurrencyType.FortuneTokens:
                            player.Client.SendMessage(new BUYRESULT
                            {
                                Result = BUY_NO_FORTUNETOKENS,
                                Message = "{\"key\":\"server.not_enough_fortunetokens\"}"
                            });
                            break;
                    }
                }
            };
        }

        protected override bool TryDeduct(Player player)
        {
            var acc = player.Client.Account;
            if (player.Stars < RankReq)
                return false;

            if (Currency == CurrencyType.Fame)
                if (acc.Fame < Price)
                    return false;

            if (Currency == CurrencyType.Gold)
                if (acc.Credits < Price)
                    return false;

            if (Currency == CurrencyType.FortuneTokens)
                if (acc.FortuneTokens < Price)
                    return false;
            return true;
        }
    }
}