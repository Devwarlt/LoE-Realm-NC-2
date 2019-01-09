using System;
using System.Collections.Generic;
using LoESoft.Core.config;

namespace LoESoft.AppEngine.dailyLogin
{
    internal class MonthCalendarUtils : fetchCalendar
    {
        internal static DateTime MonthDate = new DateTime(2019, 1, 1, 0, 0, 0, Settings.DateTimeKind);
        internal static bool DISABLE_CALENDAR = true;
        internal static List<FetchCalendarDay> MonthCalendarList = new List<FetchCalendarDay>(1)
        {
            new FetchCalendarDay
            {
                Item = 0x575a, //Public Arena Key
                Quantity = 1
            }
        };
    }
}
