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
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseYearDividedBy100(string input)
        {
            var now = DateTime.Now;
            var intYear = now.Year / 100;
            var yearInput = int.Parse(input, CultureInfo.InvariantCulture);
            while (intYear != yearInput)
            {
                now = now.AddYears(yearInput < intYear ? -100 : 100);
                intYear = now.Year / 100;
            }

            return now.Year;
        }

        public static string ConsumeHour24(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static string ParseAmPmDesignation(string input)
        {
            var chars = input.Substring(0, 2).ToUpper();
            return chars switch
            {
                "AM" => "AM",
                "PM" => "PM",
                _ => throw new ArgumentException("Invalid AM/PM designation")
            };
        }

        public static int ParseHour24(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 0 and <= 23 => res,
                _ => throw new ArgumentException("Invalid 24 hour format")
            };
        }

        public static int ParseHour12(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 1 and <= 12 => res,
                _ => throw new ArgumentException("Invalid 12 hour format")
            };
        }

        public static string ConsumeAmPmDesignation(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static string ConsumeHour12(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static string ConsumeDayOfYear(string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 3);
            inputIndex += 3;
            return res;
        }

        public static int ParseDayOfYear(string dayOfYear)
        {
            var res = int.Parse(dayOfYear.Substring(0, 3), CultureInfo.InvariantCulture);
            return res switch
            {
                >= 1 and <= 366 => res,
                _ => throw new FormatException("Invalid day of year")
            };
        }

        public static string ConsumeMonth(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseMonth(string input)
        {
            var res = int.Parse(input.Substring(0, 2), CultureInfo.InvariantCulture);
            return res switch
            {
                >= 1 and <= 12 => res,
                _ => throw new FormatException("Invalid month")
            };
        }

        public static string ConsumeMinute(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseMinute(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 0 and <= 59 => res,
                _ => throw new FormatException("Invalid minute")
            };
        }

        public static string ConsumeNewLine(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static string ConsumeTab(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static string ConsumeSecond(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseSecond(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 0 and <= 60 => res,
                _ => throw new FormatException("Invalid second")
            };
        }

        public static string ConsumeIsoTime(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 8);
            inputIndex += 8;
            return res;
        }

        public static (int, int, int) ParseIsoTime(string input)
        {
            var hour = int.Parse(input.Substring(0, 2), CultureInfo.InvariantCulture);
            var minute = int.Parse(input.Substring(3, 2), CultureInfo.InvariantCulture);
            var second = int.Parse(input.Substring(6, 2), CultureInfo.InvariantCulture);

            if (hour is < 0 or > 23) throw new FormatException("Invalid iso time hour");
            if (minute is < 0 or > 59) throw new FormatException("Invalid iso time minute");
            if (second is < 0 or > 60) throw new FormatException("Invalid iso time second");
            
            return (hour, minute, second);
        }

        public static string ConsumeIsoWeekDay(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static int ParseIsoWeekDay(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            if (res is < 1 or > 7) throw new FormatException("Invalid iso week day");
            return res;
        }

        public static string ConsumeWeekDaySundayBased(string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static int ParseWeekDaySundayBased(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 0 and <= 6 => res,
                _ => throw new FormatException("Invalid week day sunday based")
            };
        }

        public static string ConsumeYearTwoDigits(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseYearTwoDigits(string input)
        {
            var res = int.Parse(input, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 0 and <= 99 => res,
                _ => throw new FormatException("Invalid year two digits")
            };
        }

        public static int ParseYear(string input)
        {
            return int.Parse(input, CultureInfo.InvariantCulture);
        }

        public static string ConsumeYearFull(ref string input, ref int inputIndex)
        {
            var res = input.Substring(inputIndex, 4);
            inputIndex += 4;
            return res;
        }
    }
}