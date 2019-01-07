using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoESoft.AppEngine.dailyLogin
{
    internal class MonthCalendar : FetchCalendar
    {
        internal static List<FetchCalendarDay> MonthCalendarList = new List<FetchCalendarDay>(1)
        {
            new FetchCalendarDay
            {
                Item = 0x575a, //Public Arena Day
                Quantity = 1
            }
        };
    }
}
