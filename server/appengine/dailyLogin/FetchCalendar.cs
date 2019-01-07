using System;
using System.Xml.Linq;

namespace LoESoft.AppEngine.dailyLogin
{
    internal class FetchCalendar : RequestHandler
    {
        protected override void HandleRequest() => new NotImplementedException();

        internal string GetXML() //TODO
        {
            var LoginRewards = new XElement("LoginRewards");
            LoginRewards.Add(new XAttribute("serverTime", 0)); //Get Server Time
            LoginRewards.Add(new XAttribute("conCurDay", 0));
            LoginRewards.Add(new XAttribute("nonconCurDay", 0));

            var nonConsecutive = new XElement("NonConsecutive");
            nonConsecutive.Add(new XAttribute("days", 0));

            for (int day = 1; day < MonthCalendar.MonthCalendarList.Count + 1; day++)
            {
                var dayCalendar = MonthCalendar.MonthCalendarList[day - 1];

                var Login = new XElement("Login");
                Login.Add(new XElement("Days", day));

                var Item = new XElement("ItemId", dayCalendar.Item);
                Item.Add(new XAttribute("quantity", dayCalendar.Quantity));
                Login.Add(Item);

                var Gold = new XElement("Gold", dayCalendar.Gold);
                Login.Add(Gold);

                var Claimed = false;
                var IsDayUnlocked = false;

                if (Claimed) //Check if day is claimed
                {
                    Login.Add(new XElement("Claimed"));
                }
                else if (IsDayUnlocked) //Check if day is unlocked (?)
                {
                    Login.Add(new XElement("key", day - 1));
                }
                nonConsecutive.Add(Login);
            }

            LoginRewards.Add(nonConsecutive);

            var Consecutive = new XElement("Consecutive");
            Consecutive.Add(new XAttribute("days", 0));
            LoginRewards.Add(Consecutive);

            var UnlockableDays = new XElement("Unlockable");
            UnlockableDays.Add(new XAttribute("days", 0));
            LoginRewards.Add(UnlockableDays);

            return LoginRewards.ToString();
        }

        internal bool GetNextDay => false; //Use DateTime.Today and save it then :thinking:
        internal bool GetDataBase => false; //Get db pieces (?)

        internal class FetchCalendarDay
        {
            public int Item = -1;
            public int Gold = 0;
            public int Quantity = 0;
        }
    }
}
