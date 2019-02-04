#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer
{
    public class DateProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!(arg is DateTime))
                throw new NotSupportedException();

            DateTime dt = (DateTime)arg;

            string suffix, _suffix;

            if (new[] { 11, 12, 13 }.Contains(dt.Day))
                suffix = "th";
            else if (dt.Day % 10 == 1)
                suffix = "st";
            else if (dt.Day % 10 == 2)
                suffix = "nd";
            else if (dt.Day % 10 == 3)
                suffix = "rd";
            else
                suffix = "th";

            string[] time = dt.TimeOfDay.ToString().Split('.')[0].Split(':');

            _suffix = Convert.ToInt32(time[0]) >= 12 ? "PM" : "AM";

            return $"{time[0]}:{time[1]} {_suffix} {dt.Day}{suffix} {arg:MMM} {arg:yyyy}";
        }
    }

    public static class EnumerableUtils
    {
        public static T RandomElement<T>(this IEnumerable<T> source, Random rng)
        {
            T current = default(T);
            int count = 0;
            foreach (T element in source)
            {
                count++;
                if (rng.Next(count) == 0)
                {
                    current = element;
                }
            }
            if (count == 0)
            {
                throw new InvalidOperationException("Sequence was empty");
            }
            return current;
        }
    }

    public static class StringUtils
    {
        public static bool ContainsIgnoreCase(this string self, string val) => self.IndexOf(val, StringComparison.InvariantCultureIgnoreCase) != -1;

        public static bool EqualsIgnoreCase(this string self, string val) => self.Equals(val, StringComparison.InvariantCultureIgnoreCase);
    }

    public static class MathsUtils
    {
        public static double Dist(double x1, double y1, double x2, double y2) => Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        public static double DistSqr(double x1, double y1, double x2, double y2) => (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

        public static double NextDouble(this Random rand, double minValue, double maxValue) => rand.NextDouble() * (maxValue - minValue) + minValue;

        public static List<T> Clone<T>(this List<T> list) => new List<T>(list);
    }
}