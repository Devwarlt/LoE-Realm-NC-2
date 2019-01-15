using LoESoft.Core;
using LoESoft.Core.config;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LoESoft.AppEngine.dailyLogin
{
    public class MonthCalendarUtils
    {
        public static DateTime StartDate = new DateTime(2019, 1, 14, 0, 0, 0, Settings.DateTimeKind);
        public static DateTime EndDate = new DateTime(2019, 1, 25, 0, 0, 0, Settings.DateTimeKind);
        public static bool DISABLE_CALENDAR = false;

        public static List<FetchCalendarDay> MonthCalendarList = new List<FetchCalendarDay>()
        {
            new FetchCalendarDay
            {
                Item = 0x1088,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0x1188,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0xb0b,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0xaff,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0xaf6,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0xc50,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0xb08,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Item = 0xb02,
                Quantity = 1
            },
            new FetchCalendarDay
            {
                Gold = 50
            }
        };
    }

    public class FetchCalendarDay
    {
        public int Item = -1;
        public int Gold = 0;
        public int Quantity = 0;
    }

    internal class fetchCalendar : RequestHandler
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

            var status = Database.Verify(Query["guid"], Query["password"], out acc);

            if (!(status == LoginStatus.OK))
            {
                WriteLine("<Error>Account not found</Error>");
                return;
            }

            if (MonthCalendarUtils.DISABLE_CALENDAR || DateTime.UtcNow >= MonthCalendarUtils.EndDate)
            {
                WriteLine("<Error>This feature is disabled.</Error>");
                return;
            }

            var CalendarDb = new DailyCalendar(acc);

            if (CalendarDb.IsNull || IsNextMonth(CalendarDb.LastTime))
            {
                CalendarDb = new DailyCalendar(acc)
                {
                    ClaimedDays = new int[] { },
                    ConsecutiveDays = 1,
                    NonConsecutiveDays = 1,
                    UnlockableDays = 1,
                    LastTime = DateTime.Today
                };
            }

            if (IsNextDay(CalendarDb.LastTime))
            {
                CalendarDb.NonConsecutiveDays++;
                CalendarDb.UnlockableDays++;
                CalendarDb.LastTime = DateTime.Today;
            }

            CalendarDb.FlushAsync();

            WriteLine(GetXML(CalendarDb));
        }

        internal string GetXML(DailyCalendar calendar)
        {
            var LoginRewards = new XElement("LoginRewards");
            LoginRewards.Add(new XAttribute("serverTime", new DateTime(0, Settings.DateTimeKind))); //Get Server Time
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
                    Login.Add(new XElement("key", day));
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

        internal bool IsNextMonth(DateTime dateTime) => (dateTime.Year == MonthCalendarUtils.StartDate.Year && dateTime.Month != MonthCalendarUtils.StartDate.Month) || dateTime.Year != MonthCalendarUtils.StartDate.Year;
    }
}