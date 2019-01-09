#region

using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using LoESoft.AppEngine.dailyLogin;
using LoESoft.Core;
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
            int Type = Convert.ToInt32(message.Type); // probably for NonCon and Con TODO

            #endregion

            if (MonthCalendarUtils.DISABLE_CALENDAR || CalendarDb.IsNull ||
                CalendarDb.ClaimedDays.ToCommaSepString().Contains(message.ClaimKey) 
                || CalendarDb.UnlockableDays < ClaimKey || ClaimKey > Calendar.Count)
                return;

            client.SendMessage(new LOGIN_REWARD_MSG
            {
                ItemId = Calendar[ClaimKey].Item,
                Quantity = Calendar[ClaimKey].Quantity,
                Gold = Calendar[ClaimKey].Gold
            });

            if (Calendar[ClaimKey].Gold > 0)
                client.Account.Credits += Calendar[ClaimKey].Gold;
            else if(Calendar[ClaimKey].Item != -1 && Calendar[ClaimKey].Quantity > 0)
            {
                List<int> items = client.Account.Gifts.ToList();

                for(int i = 0; i < Calendar[ClaimKey].Quantity; i++)
                    items.Add(Calendar[ClaimKey].Item);

                client.Account.Gifts = items.ToArray();
            }

            var ClaimedDays = CalendarDb.ClaimedDays.ToList();

            ClaimedDays.Add(ClaimKey);
            CalendarDb.ClaimedDays = ClaimedDays.ToArray();
            CalendarDb.Flush();

            client.Account.Flush();
            client.Account.Reload();
        }
    }
}
