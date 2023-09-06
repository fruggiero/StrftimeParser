using System;
using System.Globalization;

namespace StrftimeParser
{
    internal abstract class Formatter
    {
        protected abstract CultureInfo Culture { get; }

        public virtual string ConsumeDayOfWeek(ref string input, ref int inputIndex)
        {
            foreach (var dayName in Culture.DateTimeFormat.DayNames)
            {
                if (input.Length < dayName.Length + inputIndex
                    || input.Substring(inputIndex, dayName.Length) != dayName)
                {
                    continue;
                }

                inputIndex += dayName.Length;
                return dayName;
            }

            throw new FormatException("Invalid day of week for this culture");
        }
        
        public virtual string ConsumeAbbreviatedDayOfWeek(ref string input, ref int inputIndex)
        {
            foreach (var abbreviatedDayName in Culture.DateTimeFormat.AbbreviatedDayNames)
            {
                if (input.Length < abbreviatedDayName.Length + inputIndex
                    || input.Substring(inputIndex, abbreviatedDayName.Length) != abbreviatedDayName)
                {
                    continue;
                }

                inputIndex += abbreviatedDayName.Length;
                return abbreviatedDayName;
            }

            throw new FormatException("Invalid abbreviated day of week for this culture");
        }

        public virtual string ConsumeAbbreviatedMonth(ref string input, ref int inputIndex)
        {
            foreach (var abbreviatedMonth in Culture.DateTimeFormat.AbbreviatedMonthNames)
            {
                if (input.Length < abbreviatedMonth.Length + inputIndex
                    || input.Substring(inputIndex, abbreviatedMonth.Length) != abbreviatedMonth)
                {
                    continue;
                }

                inputIndex += abbreviatedMonth.Length;
                return abbreviatedMonth;
            }

            throw new FormatException("Invalid abbreviated month for this culture");
        }

        public virtual string ConsumeFullMonth(ref string input, ref int inputIndex)
        {
            foreach (var month in Culture.DateTimeFormat.MonthNames)
            {
                if (input.Length < month.Length + inputIndex
                    || input.Substring(inputIndex, month.Length) != month)
                {
                    continue;
                }

                inputIndex += month.Length;
                return month;
            }

            throw new FormatException("Invalid month for this culture");
        }

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

        public virtual int ParseMonthFull(string input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.MonthNames.Length; i++)
            {
                if (input == Culture.DateTimeFormat.MonthNames[i])
                    return i + 1;
            }
            throw new FormatException("Invalid month for this culture");
        }

        public virtual int ParseMonthAbbreviated(string input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.AbbreviatedMonthNames.Length; i++)
            {
                if (input == Culture.DateTimeFormat.AbbreviatedMonthNames[i])
                    return i + 1;
            }
            throw new FormatException("Invalid abbreviated month for this culture");
        }

        public virtual DayOfWeek ParseDayOfWeekAbbreviated(string input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.AbbreviatedDayNames.Length; i++)
            {
                if (input == Culture.DateTimeFormat.AbbreviatedDayNames[i])
                    return (DayOfWeek)i;
            }
            throw new FormatException("Invalid abbreviated day of week for this culture");
        }

        public virtual DayOfWeek ParseDayOfWeekFull(string input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.DayNames.Length; i++)
            {
                if (input == Culture.DateTimeFormat.DayNames[i])
                    return (DayOfWeek)i;
            }
            throw new FormatException("Invalid day of week for this culture");
        }
        
        public DateTime ToDayOfWeek(DateTime source, DayOfWeek dayOfWeek)
        {
            var now = source;
            var firstDayOfWeek = Culture.DateTimeFormat.FirstDayOfWeek;
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

        public static int ParseDayOfTheMonth(string dayOfTheMonth)
        {
            var res = int.Parse(dayOfTheMonth, CultureInfo.InvariantCulture);
            return res switch
            {
                >= 1 and <= 31 => res,
                _ => throw new FormatException("Invalid day of the month")
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

        public string ToStringYearFull(DateTime dt)
        {
            return dt.ToString("yyyy", Culture);
        }

        public string ToStringYearTwoDigits(DateTime dt)
        {
            return dt.ToString("yy", Culture);
        }

        public string ToStringSecond(DateTime dt)
        {
            return dt.ToString("ss", Culture);
        }

        public string ToStringTab(DateTime _)
        {
            return "\t";
        }

        public string ToStringIsoTime(DateTime dt)
        {
            return $"{ToStringHour24(dt)}:{ToStringMinute(dt)}:{ToStringSecond(dt)}";
        }

        public static string ToStringIsoWeekDay(DateTime dt)
        {
            return dt.DayOfWeek switch
            {
                DayOfWeek.Monday => "1",
                DayOfWeek.Tuesday => "2",
                DayOfWeek.Wednesday => "3",
                DayOfWeek.Thursday => "4",
                DayOfWeek.Friday => "5",
                DayOfWeek.Saturday => "6",
                DayOfWeek.Sunday => "7",
                _ => throw new FormatException("Unrecognized day of week")
            };
        }

        public static string ToStringNewLine(DateTime dt)
        {
            return "\n";
        }

        public string ToStringMonth(DateTime dt)
        {
            return dt.ToString("MM", Culture);
        }

        public string ToStringMinute(DateTime dt)
        {
            return dt.ToString("mm", Culture);
        }

        public string ToStringHour12(DateTime dt)
        {
            return dt.ToString("hh", Culture);
        }

        public static string ToStringAmPmDesignation(DateTime dt)
        {
            return dt.Hour switch
            {
                >=0 and <= 11 => "AM",
                > 11 and <= 23 => "PM",
                _ => throw new FormatException("Unrecognized hour number")
            };
        }

        public string ToStringHour24(DateTime dt)
        {
            return dt.ToString("HH", Culture);
        }

        public string ToStringFullMonthName(DateTime dt)
        {
            return dt.ToString("MMMM", Culture);
        }

        public string ToStringAbbreviatedMonthName(DateTime dt)
        {
            return dt.ToString("MMM", Culture);
        }

        public string ToStringYearDividedBy100(DateTime dt)
        {
            return (dt.Year / 100).ToString(Culture);
        }

        public string ToStringDateAndTime(DateTime dt)
        {
            return dt.ToString("ddd MMM dd HH:mm:ss yyyy", Culture);
        }

        public string ToStringAbbreviatedWeekDayName(DateTime dt)
        {
            return dt.ToString("ddd", Culture);
        }

        public string ToStringFullWeekDayName(DateTime dt)
        {
            return dt.ToString("dddd", Culture);
        }

        public string ToStringDaySpacePadded(DateTime dt)
        {
            return dt.Day.ToString("d", Culture).PadLeft(2, ' ');
        }

        public string ToStringDayZeroPadded(DateTime dt)
        {
            return dt.ToString("dd", Culture);
        }

        public string ToStringShortMmDdYy(DateTime dt)
        {
            return $"{ToStringMonth(dt)}/{ToStringDayZeroPadded(dt)}/{ToStringYearTwoDigits(dt)}";
        }

        public string ToStringShortYyyyMmDd(DateTime dt)
        {
            return $"{ToStringYearFull(dt)}-{ToStringMonth(dt)}-{ToStringDayZeroPadded(dt)}";
        }

        public string ToStringDayOfTheYear(DateTime dt)
        {
            return dt.DayOfYear.ToString(Culture).PadLeft(3, '0');
        }

        public string ToStringWeekDaySundayBased(DateTime dt)
        {
            return dt.DayOfWeek switch
            {
                DayOfWeek.Sunday => "0",
                DayOfWeek.Monday => "1",
                DayOfWeek.Tuesday => "2",
                DayOfWeek.Wednesday => "3",
                DayOfWeek.Thursday => "4",
                DayOfWeek.Friday => "5",
                DayOfWeek.Saturday => "6",
                _ => throw new FormatException("Unrecognized day of week")
            };
        }
    }
}