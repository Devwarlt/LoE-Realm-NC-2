#region

using LoESoft.AppEngine.dailyLogin;
using LoESoft.Core;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.networking.handlers
{
    internal class ClaimDailyReward : MessageHandlers<CLAIM_LOGIN_REWARD_MSG>
    {
        public override MessageID ID => MessageID.CLAIM_LOGIN_REWARD_MSG;

        protected override void HandleMessage(Client client, CLAIM_LOGIN_REWARD_MSG message)
        {
            DailyCalendar CalendarDb = new DailyCalendar(client.Account);
            var Calendar = MonthCalendarUtils.MonthCalendarList;
            #region Message vars

            int ClaimKey = (Convert.ToInt32(message.ClaimKey) - 1);
            string Type = message.Type; // TODO Consecutive Calendar

            #endregion

            if (MonthCalendarUtils.DISABLE_CALENDAR || DateTime.UtcNow >= MonthCalendarUtils.EndDate || CalendarDb.IsNull ||
                CalendarDb.ClaimedDays.ToCommaSepString().Contains(message.ClaimKey + 1)
                || CalendarDb.UnlockableDays < ClaimKey || Calendar.Count < ClaimKey)
                return;

            client.SendMessage(new LOGIN_REWARD_MSG
            {
                ItemId = Calendar[ClaimKey].Item,
                Quantity = Calendar[ClaimKey].Quantity,
                Gold = Calendar[ClaimKey].Gold
            });

            if (Calendar[ClaimKey].Gold > 0)
                client.Account.Credits += Calendar[ClaimKey].Gold;
            else if (Calendar[ClaimKey].Item != -1 && Calendar[ClaimKey].Quantity > 0)
            {
                List<int> items = client.Account.Gifts.ToList();

                for (int i = 0; i < Calendar[ClaimKey].Quantity; i++)
                    items.Add(Calendar[ClaimKey].Item);

                client.Account.Gifts = items.ToArray();
            }

            var ClaimedDays = CalendarDb.ClaimedDays.ToList();

            ClaimedDays.Add(ClaimKey + 1);
            CalendarDb.ClaimedDays = ClaimedDays.ToArray();
            CalendarDb.FlushAsync();

            client.Account.FlushAsync();
            client.Account.Reload();
        }
    }
}