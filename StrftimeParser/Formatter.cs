using System;
using System.Globalization;

namespace StrftimeParser
{
    internal abstract class Formatter
    {
        protected abstract CultureInfo Culture { get; }

        public virtual ReadOnlySpan<char> ConsumeDayOfWeek(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            foreach (var dayName in Culture.DateTimeFormat.DayNames)
            {
                if (input.Length < dayName.Length + inputIndex)
                    continue;

                var inputSlice = input.Slice(inputIndex, dayName.Length);
                if (!inputSlice.SequenceEqual(dayName.AsSpan()))
                    continue;

                var result = inputSlice;
                inputIndex += dayName.Length;
                return result;
            }

            throw new FormatException("Invalid day of week for this culture");
        }
        
        public virtual ReadOnlySpan<char> ConsumeAbbreviatedDayOfWeek(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            foreach (var abbreviatedDayName in Culture.DateTimeFormat.AbbreviatedDayNames)
            {
                if (input.Length < abbreviatedDayName.Length + inputIndex)
                    continue;

                var inputSlice = input.Slice(inputIndex, abbreviatedDayName.Length);
                if (!inputSlice.SequenceEqual(abbreviatedDayName.AsSpan()))
                    continue;

                var result = inputSlice;
                inputIndex += abbreviatedDayName.Length;
                return result;
            }

            throw new FormatException("Invalid abbreviated day of week for this culture");
        }

        public virtual ReadOnlySpan<char> ConsumeAbbreviatedMonth(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            foreach (var abbreviatedMonth in Culture.DateTimeFormat.AbbreviatedMonthNames)
            {
                if (input.Length < abbreviatedMonth.Length + inputIndex)
                    continue;

                var inputSlice = input.Slice(inputIndex, abbreviatedMonth.Length);
                if (!inputSlice.SequenceEqual(abbreviatedMonth.AsSpan()))
                    continue;

                var result = inputSlice;
                inputIndex += abbreviatedMonth.Length;
                return result;
            }

            throw new FormatException("Invalid abbreviated month for this culture");
        }

        
        public virtual ReadOnlySpan<char> ConsumeFullMonth(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            foreach (var month in Culture.DateTimeFormat.MonthNames)
            {
                if (input.Length < month.Length + inputIndex)
                    continue;

                var inputSlice = input.Slice(inputIndex, month.Length);
                if (!inputSlice.SequenceEqual(month.AsSpan()))
                    continue;

                var result = inputSlice;
                inputIndex += month.Length;
                return result;
            }

            throw new FormatException("Invalid month for this culture");
        }

        public static ReadOnlySpan<char> ConsumeDayOfTheMonth(ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public virtual int ParseMonthFull(ReadOnlySpan<char> input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.MonthNames.Length; i++)
            {
                if (input.SequenceEqual(Culture.DateTimeFormat.MonthNames[i].AsSpan()))
                    return i + 1;
            }
            throw new FormatException("Invalid month for this culture");
        }

        public virtual int ParseMonthAbbreviated(ReadOnlySpan<char> input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.AbbreviatedMonthNames.Length; i++)
            {
                if (input.SequenceEqual(Culture.DateTimeFormat.AbbreviatedMonthNames[i].AsSpan()))
                    return i + 1;
            }
            throw new FormatException("Invalid abbreviated month for this culture");
        }

        public virtual DayOfWeek ParseDayOfWeekAbbreviated(ReadOnlySpan<char> input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.AbbreviatedDayNames.Length; i++)
            {
                if (input.SequenceEqual(Culture.DateTimeFormat.AbbreviatedDayNames[i].AsSpan()))
                    return (DayOfWeek)i;
            }
            throw new FormatException("Invalid abbreviated day of week for this culture");
        }

        public virtual DayOfWeek ParseDayOfWeekFull(ReadOnlySpan<char> input)
        {
            for (int i = 0; i < Culture.DateTimeFormat.DayNames.Length; i++)
            {
                if (input.SequenceEqual(Culture.DateTimeFormat.DayNames[i].AsSpan()))
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

        public static ReadOnlySpan<char> ConsumeHour24(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static string ParseAmPmDesignation(ReadOnlySpan<char> input)
        {
            if (input.Length < 2)
                throw new ArgumentException("Invalid AM/PM designation");

            char c1 = char.ToUpper(input[0]);
            char c2 = char.ToUpper(input[1]);

            if (c1 == 'A' && c2 == 'M')
                return "AM";
            else if (c1 == 'P' && c2 == 'M')
                return "PM";
            else
                throw new ArgumentException("Invalid AM/PM designation");
        }

        public static int ParseHour24(ReadOnlySpan<char> input)
        {
            if (input.Length is < 1 or > 2)
                throw new ArgumentException("Invalid 24 hour format");

            var result = 0;
            foreach (var c in input)
            {
                if (c is < '0' or > '9')
                    throw new ArgumentException("Invalid 24 hour format");
                result = result * 10 + (c - '0');
            }

            return result switch
            {
                >= 0 and <= 23 => result,
                _ => throw new ArgumentException("Invalid 24 hour format")
            };
        }

        public static int ParseHour12(ReadOnlySpan<char> input)
        {
            if (input.Length is < 1 or > 2)
                throw new ArgumentException("Invalid 12 hour format");

            var result = 0;
            foreach (var c in input)
            {
                if (c is < '0' or > '9')
                    throw new ArgumentException("Invalid 12 hour format");
                result = result * 10 + (c - '0');
            }

            return result switch
            {
                >= 1 and <= 12 => result,
                _ => throw new ArgumentException("Invalid 12 hour format")
            };
        }

        public static ReadOnlySpan<char> ConsumeAmPmDesignation(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static ReadOnlySpan<char> ConsumeHour12(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static ReadOnlySpan<char> ConsumeDayOfYear(ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 3);
            inputIndex += 3;
            return res;
        }

        public static int ParseDayOfYear(ReadOnlySpan<char> dayOfYear)
        {
            if (dayOfYear.Length < 3)
                throw new FormatException("Invalid day of year");

            char c1 = dayOfYear[0];
            char c2 = dayOfYear[1];
            char c3 = dayOfYear[2];
    
            if (c1 < '0' || c1 > '9' || c2 < '0' || c2 > '9' || c3 < '0' || c3 > '9')
                throw new FormatException("Invalid day of year");

            int result = (c1 - '0') * 100 + (c2 - '0') * 10 + (c3 - '0');

            return result switch
            {
                >= 1 and <= 366 => result,
                _ => throw new FormatException("Invalid day of year")
            };
        }

        public static int ParseDayOfTheMonthZeroPadded(ReadOnlySpan<char> dayOfTheMonth)
        {
            if (dayOfTheMonth.Length is < 1 or > 2)
                throw new FormatException("Invalid day of the month");

            var result = 0;
            foreach (var c in dayOfTheMonth)
            {
                if (c is < '0' or > '9')
                    throw new FormatException("Invalid day of the month");
                result = result * 10 + (c - '0');
            }

            return result switch
            {
                >= 1 and <= 31 => result,
                _ => throw new FormatException("Invalid day of the month")
            };
        }
        
        public static int ParseDayOfTheMonthFlexible(ReadOnlySpan<char> dayOfTheMonth)
        {
            if (dayOfTheMonth.Length is < 1 or > 2)
                throw new FormatException("Invalid day of the month");

            var result = 0;
            
            // Handle first character
            char firstChar = dayOfTheMonth[0];
            if (firstChar == ' ')
            {
                // Space-padded single digit - first char is space, ignore it
                result = 0;
            }
            else if (firstChar is >= '0' and <= '9')
            {
                // Valid digit
                result = firstChar - '0';
            }
            else
            {
                throw new FormatException("Invalid day of the month");
            }
            
            // Handle second character if present
            if (dayOfTheMonth.Length == 2)
            {
                char secondChar = dayOfTheMonth[1];
                if (secondChar is >= '0' and <= '9')
                {
                    result = result * 10 + (secondChar - '0');
                }
                else
                {
                    throw new FormatException("Invalid day of the month");
                }
            }

            return result switch
            {
                >= 1 and <= 31 => result,
                _ => throw new FormatException("Invalid day of the month")
            };
        }

        public static int ParseDayOfTheMonthSpacePadded(ReadOnlySpan<char> dayOfTheMonth)
        {
            if (dayOfTheMonth.Length != 2)
                throw new FormatException("Invalid space-padded day of the month");

            int result = 0;

            // Handle the first character (can be space or digit)
            char firstChar = dayOfTheMonth[0];
            if (firstChar == ' ')
            {
                // Space-padded single digit (e.g., " 1", " 9")
                result = 0;
            }
            else if (firstChar is >= '0' and <= '9')
            {
                // Two-digit number (e.g., "10", "31")
                result = firstChar - '0';
            }
            else
            {
                throw new FormatException("Invalid space-padded day of the month");
            }

            // Handle the second character (must be a digit)
            char secondChar = dayOfTheMonth[1];
            if (secondChar is >= '0' and <= '9')
            {
                result = result * 10 + (secondChar - '0');
            }
            else
            {
                throw new FormatException("Invalid space-padded day of the month");
            }

            return result switch
            {
                >= 1 and <= 31 => result,
                _ => throw new FormatException("Invalid space-padded day of the month")
            };
        }
        
        public static ReadOnlySpan<char> ConsumeMonth(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseMonth(ReadOnlySpan<char> input)
        {
            if (input.Length < 2)
                throw new FormatException("Invalid month");

            char c1 = input[0];
            char c2 = input[1];
    
            if (c1 < '0' || c1 > '9' || c2 < '0' || c2 > '9')
                throw new FormatException("Invalid month");

            int result = (c1 - '0') * 10 + (c2 - '0');

            return result switch
            {
                >= 1 and <= 12 => result,
                _ => throw new FormatException("Invalid month")
            };
        }

        public static ReadOnlySpan<char> ConsumeMinute(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseMinute(ReadOnlySpan<char> input)
        {
            if (input.Length is < 1 or > 2)
                throw new FormatException("Invalid minute");

            var result = 0;
            foreach (var c in input)
            {
                if (c is < '0' or > '9')
                    throw new FormatException("Invalid minute");
                result = result * 10 + (c - '0');
            }

            return result switch
            {
                >= 0 and <= 59 => result,
                _ => throw new FormatException("Invalid minute")
            };
        }

        public static ReadOnlySpan<char> ConsumeNewLine(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static ReadOnlySpan<char> ConsumeTab(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static ReadOnlySpan<char> ConsumeSecond(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseSecond(ReadOnlySpan<char> input)
        {
            if (input.Length is < 1 or > 2)
                throw new FormatException("Invalid second");

            var result = 0;
            foreach (var c in input)
            {
                if (c is < '0' or > '9')
                    throw new FormatException("Invalid second");
                result = result * 10 + (c - '0');
            }

            return result switch
            {
                >= 0 and <= 60 => result,
                _ => throw new FormatException("Invalid second")
            };
        }

        public static ReadOnlySpan<char> ConsumeIsoTime(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 8);
            inputIndex += 8;
            return res;
        }

        public static (int, int, int) ParseIsoTime(ReadOnlySpan<char> input)
        {
            if (input.Length != 8)
                throw new FormatException("Invalid iso time format");

            // Parse hour (HH)
            char h1 = input[0];
            char h2 = input[1];
            if (h1 < '0' || h1 > '9' || h2 < '0' || h2 > '9')
                throw new FormatException("Invalid iso time hour");
            int hour = (h1 - '0') * 10 + (h2 - '0');

            // Check colon separator
            if (input[2] != ':')
                throw new FormatException("Invalid iso time format");

            // Parse minute (MM)
            char m1 = input[3];
            char m2 = input[4];
            if (m1 < '0' || m1 > '9' || m2 < '0' || m2 > '9')
                throw new FormatException("Invalid iso time minute");
            int minute = (m1 - '0') * 10 + (m2 - '0');

            // Check colon separator
            if (input[5] != ':')
                throw new FormatException("Invalid iso time format");

            // Parse second (SS)
            char s1 = input[6];
            char s2 = input[7];
            if (s1 < '0' || s1 > '9' || s2 < '0' || s2 > '9')
                throw new FormatException("Invalid iso time second");
            int second = (s1 - '0') * 10 + (s2 - '0');

            // Validate ranges
            if (hour is < 0 or > 23) throw new FormatException("Invalid iso time hour");
            if (minute is < 0 or > 59) throw new FormatException("Invalid iso time minute");
            if (second is < 0 or > 60) throw new FormatException("Invalid iso time second");
    
            return (hour, minute, second);
        }

        public static ReadOnlySpan<char> ConsumeIsoWeekDay(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        
        public static int ParseIsoWeekDay(ReadOnlySpan<char> input)
        {
            if (input.Length != 1)
                throw new FormatException("Invalid iso week day");

            char c = input[0];
            if (c is < '0' or > '9')
                throw new FormatException("Invalid iso week day");

            int result = c - '0';
    
            if (result is < 1 or > 7) 
                throw new FormatException("Invalid iso week day");
        
            return result;
        }

        public static ReadOnlySpan<char> ConsumeWeekDaySundayBased(ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 1);
            inputIndex += 1;
            return res;
        }

        public static int ParseWeekDaySundayBased(ReadOnlySpan<char> input)
        {
            if (input.Length != 1)
                throw new FormatException("Invalid week day sunday based");

            char c = input[0];
            if (c is < '0' or > '9')
                throw new FormatException("Invalid week day sunday based");

            int result = c - '0';
    
            return result switch
            {
                >= 0 and <= 6 => result,
                _ => throw new FormatException("Invalid week day sunday based")
            };
        }

        public static ReadOnlySpan<char> ConsumeYearTwoDigits(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 2);
            inputIndex += 2;
            return res;
        }

        public static int ParseYearTwoDigits(ReadOnlySpan<char> input)
        {
            if (input.Length != 2)
                throw new FormatException("Invalid year two digits");

            char c1 = input[0];
            char c2 = input[1];
    
            if (c1 < '0' || c1 > '9' || c2 < '0' || c2 > '9')
                throw new FormatException("Invalid year two digits");

            int result = (c1 - '0') * 10 + (c2 - '0');
    
            return result switch
            {
                >= 0 and <= 99 => result,
                _ => throw new FormatException("Invalid year two digits")
            };
        }

        public static int ParseYear(ReadOnlySpan<char> input)
        {
            // Manual parsing for 4-digit year (YYYY)
            if (input.Length != 4)
                throw new FormatException("Invalid year format");

            var result = 0;
            for (var i = 0; i < 4; i++)
            {
                var c = input[i];
                if (c is < '0' or > '9')
                    throw new FormatException("Invalid year format");
                result = result * 10 + (c - '0');
            }
            return result;
        }

        public static ReadOnlySpan<char> ConsumeYearFull(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var res = input.Slice(inputIndex, 4);
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

        protected static bool CheckStringMatch(ReadOnlySpan<char> input, int startIndex, string target)
        {
            if (startIndex + target.Length > input.Length)
                return false;

            var slice = input.Slice(startIndex, target.Length);

            for (int i = 0; i < target.Length; i++)
            {
                if ((slice[i] | 0x20) != target[i])
                    return false;
            }

            return true;
        }
    }
}