#region

using System.Collections.Generic;

#endregion

namespace LoESoft.GameServer.realm.entity.merchant
{
    partial class Merchant
    {
        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.MERCHANDISE_TYPE_STAT] = MType;
            stats[StatsType.MERCHANDISE_COUNT_STAT] = MRemaining;
            stats[StatsType.MERCHANDISE_MINS_LEFT_STAT] = newMerchant ? int.MaxValue : MTime;
            stats[StatsType.MERCHANDISE_DISCOUNT_STAT] = 0;
            stats[StatsType.MERCHANDISE_PRICE_STAT] = Price;
            stats[StatsType.MERCHANDISE_RANK_REQ_STAT] = RankReq;
            stats[StatsType.MERCHANDISE_CURRENCY_STAT] = Currency;

            base.ExportStats(stats);
        }
    }
}