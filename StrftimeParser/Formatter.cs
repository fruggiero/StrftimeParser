using System;
using System.Globalization;

namespace StrftimeParser
{
    internal abstract class Formatter
    {
        public abstract string ConsumeDayOfWeek(ref string input, ref int inputIndex);
        public abstract string ConsumeAbbreviatedDayOfWeek(ref string input, ref int inputIndex);
        public abstract string ConsumeAbbreviatedMonth(ref string input, ref int inputIndex);
        public abstract string ConsumeFullMonth(ref string input, ref int inputIndex);
        public static string ConsumeDayOfTheMonth(string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static string ConsumeShortMmDdYy(string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 8);
            inputIndex += 8;
            return res;
        }

        public static string ConsumeShortYyyyMmDd(string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 10);
            inputIndex += 10;
            return res;
        }

        public static MmDdYy ParseShortMmDdYy(string input)
        {
            return new MmDdYy
            {
                Mm = int.Parse(input.Substring(0, 2)),
                Dd = int.Parse(input.Substring(3, 2)),
                Yy = int.Parse(input.Substring(6, 2))
            };
        }

        public static YyyyMmDd ParseShortYyyyMmDd(string input)
        {
            return new YyyyMmDd
            {
                Yyyy = int.Parse(input.Substring(0, 4)),
                Mm = int.Parse(input.Substring(5, 2)),
                Dd = int.Parse(input.Substring(8, 2))
            };
        }
        
        public abstract int ParseMonthFull(string input);
        public abstract int ParseMonthAbbreviated(string input);
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

        public static DateTime ToMonth(DateTime source, int month)
        {
            var now = source;
            while (now.Month != 1)
            {
                now = now.AddMonths(-1);
            }
            while (now.Month != month)
            {
                now = now.AddMonths(1);
            }

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

        public static string ConsumeYearDividedBy100(ref string input, ref int inputIndex)
        {
            var res = input.Substring(0, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseYearDividedBy100(string input)
        {
            var now = DateTime.Now;
            var intYear = now.Year / 100;
            var yearInput = int.Parse(input);
            while (intYear != yearInput)
            {
                now = now.AddYears(yearInput < intYear ? -100 : 100);
                intYear = now.Year / 100;
            }

            return now.Year;
        }
    }
}