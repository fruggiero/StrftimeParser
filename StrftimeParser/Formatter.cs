using System;
using System.Globalization;

namespace StrftimeParser
{
    internal abstract class Formatter
    {
        public abstract string ConsumeDayOfWeek(ref string input, ref int inputIndex);
        public abstract string ConsumeAbbreviatedDayOfWeek(ref string input, ref int inputIndex);

        public static string ConsumeDayOfTheMonth(string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public abstract DayOfWeek ParseDayOfWeekAbbreviated(string input);
        public abstract DayOfWeek ParseDayOfWeekFull(string input);

        public static DateTime ToDayOfWeek(DateTime source, DayOfWeek dayOfWeek)
        {
            var now = source;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            while (now.DayOfWeek != firstDayOfWeek) now = now.AddDays(-1);
            while (now.DayOfWeek != dayOfWeek) now = now.AddDays(1);
            return now;
        }

        public static DateTime ToDayOfTheMonth(DateTime source, int dayOfTheMonthNumber)
        {
            var now = source;
            if (now.Day < dayOfTheMonthNumber)
            {
                now = now.AddDays(dayOfTheMonthNumber - now.Day);
            }
            else if (now.Day > dayOfTheMonthNumber)
            {
                now = now.AddDays(-(now.Day - dayOfTheMonthNumber));
            }
            return now;
        }
    }
}