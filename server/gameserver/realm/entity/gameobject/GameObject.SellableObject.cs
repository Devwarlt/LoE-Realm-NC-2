#region

using LoESoft.Core;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    public partial class SellableObject : GameObject
    {
        private const int BUY_NO_GOLD = 3;

        public SellableObject(ushort objType)
            : base(objType, null, true, false, false)
        {
            if (objType == 0x0505) //Vault chest
            {
                Price = 2500;
                Currency = CurrencyType.Fame;
                RankReq = 0;
            }
            else if (objType == 0x0736)
            {
                Currency = CurrencyType.GuildFame;
                Price = 10000;
                RankReq = 0;
            }
        }

        public int Price { get; set; }
        public CurrencyType Currency { get; set; }
        public int RankReq { get; set; }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.MERCHANDISE_PRICE_STAT] = Price;
            stats[StatsType.MERCHANDISE_CURRENCY_STAT] = (int)Currency;
            stats[StatsType.MERCHANDISE_RANK_REQ_STAT] = RankReq;
            base.ExportStats(stats);
        }

        protected virtual bool TryDeduct(Player player)
        {
            DbAccount acc = player.Client.Account;
            if (!player.NameChosen)
                return false;
            if (player.Stars < RankReq)
                return false;

            if (Currency == CurrencyType.Fame)
                if (acc.Fame < Price)
                    return false;

            if (Currency == CurrencyType.Gold)
                if (acc.Credits < Price)
                    return false;

            return true;
        }

        public virtual void Buy(Player player)
        {
            if (ObjectType == 0x0505) //Vault chest
            {
                if (TryDeduct(player))
                {
                    GameServer.Manager.Database.UpdateFame(player.Client.Account, -Price);
                    player.CurrentFame = player.Client.Account.Fame;
                    player.UpdateCount++;
                    player.SaveToCharacter();
                    (Owner as Vault).AddChest(this);
                    player.Client.SendMessage(new BUYRESULT
                    {
                        Result = 0,
                        Message = "{\"key\":\"server.buy_success\"}"
                    });
                }
                else
                {
                    player.Client.SendMessage(new BUYRESULT
                    {
                        Result = 0,
                        Message = "{\"key\":\"server.not_enough_fame\"}"
                    });
                }
            }
            if (ObjectType == 0x0736)
            {
                player.Client.SendMessage(new BUYRESULT()
                {
                    Result = 9,
                    Message = "{\"key\":\"server.not_enough_game\"}"
                });
            }
        }
    }
}