using System;
using System.Xml.Linq;
using LoESoft.Core;

namespace LoESoft.AppEngine.dailyLogin
{
    internal class FetchCalendar : RequestHandler
    {
        /* Queries:
         *  do_login (bool) - Not Guest (?)
         *  game_net_user_id (int) - ?
         *  ignore (int) - ?
         *  game_net (string) - ?
         *  giud (string) - Account E-mail
         *  play_platform (string) - ?
         *  password (string) - Account Password
         *  gameClientVersion (string) - Client Version
        */
        protected override void HandleRequest()
        {
            DbAccount acc;
            LoginStatus status = Database.Verify(Query["guid"], Query["password"], out acc);

            if (!(status == LoginStatus.OK))
            {
                WriteLine("<Error>Account not found</Error>");
                return;
            }

            DailyCalendar CalendarDb = new DailyCalendar(acc);

            if(CalendarDb.IsNull || IsNextMonth(CalendarDb.LastTime))
            {
                CalendarDb = new DailyCalendar(acc)
                {
                    ClaimedDays = new int[] { 1 },
                    ConsecutiveDays = 1,
                    NonConsecutiveDays = 1,
                    UnlockableDays = 1,
                    LastTime = DateTime.Today
                };
            }

            if(IsNextDay(CalendarDb.LastTime))
            {
                CalendarDb.NonConsecutiveDays++;
                CalendarDb.UnlockableDays++;
                CalendarDb.LastTime = DateTime.Today;
            }

            CalendarDb.Flush();

            WriteLine(GetXML(CalendarDb));
        }

        internal string GetXML(DailyCalendar calendar)
        {
            var LoginRewards = new XElement("LoginRewards");
            LoginRewards.Add(new XAttribute("serverTime", 0)); //Get Server Time
            LoginRewards.Add(new XAttribute("conCurDay", calendar.ConsecutiveDays));
            LoginRewards.Add(new XAttribute("nonconCurDay", calendar.NonConsecutiveDays));

            var nonConsecutive = new XElement("NonConsecutive");
            nonConsecutive.Add(new XAttribute("days", calendar.NonConsecutiveDays));

            for (int day = 1; day < MonthCalendarUtils.MonthCalendarList.Count + 1; day++)
            {
                var dayCalendar = MonthCalendarUtils.MonthCalendarList[day - 1];

                var Login = new XElement("Login");
                Login.Add(new XElement("Days", day));

                var Item = new XElement("ItemId", dayCalendar.Item);
                Item.Add(new XAttribute("quantity", dayCalendar.Quantity));
                Login.Add(Item);

                var Gold = new XElement("Gold", dayCalendar.Gold);
                Login.Add(Gold);

                if (calendar.ClaimedDays.ToCommaSepString().Contains(day.ToString()))
                {
                    Login.Add(new XElement("Claimed"));
                }
                else if (calendar.UnlockableDays >= day)
                {
                    Login.Add(new XElement("key", day - 1));
                }
                nonConsecutive.Add(Login);
            }

            LoginRewards.Add(nonConsecutive);

            var Consecutive = new XElement("Consecutive");
            Consecutive.Add(new XAttribute("days", calendar.ConsecutiveDays));
            LoginRewards.Add(Consecutive);

            var UnlockableDays = new XElement("Unlockable");
            UnlockableDays.Add(new XAttribute("days", calendar.UnlockableDays));
            LoginRewards.Add(UnlockableDays);

            return LoginRewards.ToString();
        }

        internal bool IsNextDay(DateTime dateTime) => dateTime != DateTime.Today;
        internal bool IsNextMonth(DateTime dateTime) => (dateTime.Year == MonthCalendarUtils.MonthDate.Year && dateTime.Month != MonthCalendarUtils.MonthDate.Month) || dateTime.Year != MonthCalendarUtils.MonthDate.Year;

        internal class FetchCalendarDay
        {
            public int Item = -1;
            public int Gold = 0;
            public int Quantity = 0;
        }
    }
}
