using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using DateTime = System.DateTime;

namespace StrftimeParser
{
    public static class Strftime
    {
        /// <summary>
        /// Attempts to convert a strftime-formatted string into a DateTime object.
        /// For the conversion, the current culture is used.
        /// </summary>
        /// <param name="input">The string which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="result">When this method returns, contains the DateTime equivalent to the date and time contained in input, if the conversion succeeded, or DateTime.MinValue if the conversion failed.</param>
        /// <returns>true if the conversion succeeded; otherwise, false.</returns>
        public static bool TryParse(string input, string format, out DateTime result) => 
            TryParse(input, format, CultureInfo.CurrentCulture, out result);

        /// <summary>
        /// Attempts to convert a strftime-formatted string into a DateTime object.
        /// For the conversion, a specified culture is used.
        /// </summary>
        /// <param name="input">The string which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="culture">A culture-info used for the conversion</param>
        /// <param name="result">When this method returns, contains the DateTime equivalent to the date and time contained in input, if the conversion succeeded, or DateTime.MinValue if the conversion failed.</param>
        /// <param name="coherenceCheck">Throw exception on incoherence found</param>
        /// <returns>true if the conversion succeeded; otherwise, false.</returns>
        public static bool TryParse(string input, string format, CultureInfo culture, out DateTime result, bool coherenceCheck = false)
        {
            result = DateTime.MinValue;
            
            if (input == null || format == null || culture == null)
                return false;

            try
            {
                result = Parse(input, format, culture, coherenceCheck);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a strftime-formatted span into a DateTime object.
        /// For the conversion, the current culture is used.
        /// </summary>
        /// <param name="input">The span which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="result">When this method returns, contains the DateTime equivalent to the date and time contained in input, if the conversion succeeded, or DateTime.MinValue if the conversion failed.</param>
        /// <returns>true if the conversion succeeded; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out DateTime result) =>
            TryParse(input, format, CultureInfo.CurrentCulture, out result);

        /// <summary>
        /// Attempts to convert a strftime-formatted span into a DateTime object.
        /// For the conversion, a specified culture is used.
        /// </summary>
        /// <param name="input">The span which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="culture">A culture-info used for the conversion</param>
        /// <param name="result">When this method returns, contains the DateTime equivalent to the date and time contained in input, if the conversion succeeded, or DateTime.MinValue if the conversion failed.</param>
        /// <param name="coherenceCheck">Throw exception on incoherence found</param>
        /// <returns>true if the conversion succeeded; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> input, ReadOnlySpan<char> format, CultureInfo culture, out DateTime result, bool coherenceCheck = false)
        {
            result = DateTime.MinValue;
            
            if (culture == null)
                return false;

            try
            {
                result = Parse(input, format, culture, coherenceCheck);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a DateTime object to a strftime-formatted string.
        /// For the conversion, the current culture is used.
        /// </summary>
        /// <param name="dt">The DateTime object to format</param>
        /// <param name="format">A format specifier</param>
        /// <returns>A formatted date string</returns>
        /// <exception cref="ArgumentNullException">format is null</exception>
        /// <exception cref="FormatException">The format specifier is invalid</exception>
        public static string ToString(DateTime dt, string format) => ToString(dt, format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Converts a DateTime object to a strftime-formatted string.
        /// For the conversion, a specified culture is used.
        /// </summary>
        /// <param name="dt">The DateTime object to format</param>
        /// <param name="format">A format specifier</param>
        /// <param name="culture">A culture-info used for the conversion</param>
        /// <returns>A formatted date string</returns>
        /// <exception cref="ArgumentNullException">format or culture is null</exception>
        /// <exception cref="FormatException">The format specifier is invalid</exception>
        public static string ToString(DateTime dt, string format, CultureInfo culture)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (culture == null) throw new ArgumentNullException(nameof(culture));

            var formatter = Formatter.For(culture);
            var builder = new StringBuilder();

            var formatIndex = 0;
            while (formatIndex < format.Length)
            {
                if (format[formatIndex] == '%' && formatIndex + 1 < format.Length)
                {
                    formatIndex++; // Move to specifier character
                    switch (format[formatIndex])
                    {
                        case 'Y': 
                            builder.Append(formatter.ToStringYearFull(dt)); 
                            break;
                        case 'y':
                            builder.Append(formatter.ToStringYearTwoDigits(dt));
                            break;
                        case 'S':
                            builder.Append(formatter.ToStringSecond(dt));
                            break;
                        case 't':
                            builder.Append(Formatter.ToStringTab(dt));
                            break;
                        case 'T':
                            builder.Append(formatter.ToStringIsoTime(dt));
                            break;
                        case 'u':
                            builder.Append(Formatter.ToStringIsoWeekDay(dt));
                            break;
                        case 'n':
                            builder.Append(Formatter.ToStringNewLine(dt));
                            break;
                        case 'm':
                            builder.Append(formatter.ToStringMonth(dt));
                            break;
                        case 'M':
                            builder.Append(formatter.ToStringMinute(dt));
                            break;
                        case 'I':
                            builder.Append(formatter.ToStringHour12(dt));
                            break;
                        case 'p':
                            builder.Append(Formatter.ToStringAmPmDesignation(dt));
                            break;
                        case 'H':
                            builder.Append(formatter.ToStringHour24(dt));
                            break;
                        case 'B':
                            builder.Append(formatter.ToStringFullMonthName(dt));
                            break;
                        case 'b':
                            builder.Append(formatter.ToStringAbbreviatedMonthName(dt));
                            break;
                        case 'C':
                            builder.Append(formatter.ToStringYearDividedBy100(dt));
                            break;
                        case 'c':
                            builder.Append(formatter.ToStringDateAndTime(dt));
                            break;
                        case 'a':
                            builder.Append(formatter.ToStringAbbreviatedWeekDayName(dt));
                            break;
                        case 'A':
                            builder.Append(formatter.ToStringFullWeekDayName(dt));
                            break;
                        case 'd':
                            builder.Append(formatter.ToStringDayZeroPadded(dt));
                            break;
                        case 'e':
                            builder.Append(formatter.ToStringDaySpacePadded(dt));
                            break;
                        case 'D':
                            builder.Append(formatter.ToStringShortMmDdYy(dt));
                            break;
                        case 'F':
                            builder.Append(formatter.ToStringShortYyyyMmDd(dt));
                            break;
                        case 'j':
                            builder.Append(formatter.ToStringDayOfTheYear(dt));
                            break;
                        case 'w':
                            builder.Append(Formatter.ToStringWeekDaySundayBased(dt));
                            break;
                        case '%':
                            builder.Append('%');
                            break;
                        default:
                            throw new FormatException($"Unrecognized format: %{format[formatIndex]}");
                    }
                }
                else
                {
                    builder.Append(format[formatIndex]);
                }
                formatIndex++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert a strftime-formatted string into a DateTime object.
        /// For the conversion, the current culture is used.
        /// </summary>
        /// <param name="input">The string which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="ArgumentNullException">input or format is null</exception>
        /// <exception cref="FormatException">There is something wrong with the provided input</exception>
        /// <exception cref="ArgumentOutOfRangeException">There is something wrong with the formatted date</exception>
        public static DateTime Parse(string input, string format) => Parse(input, format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Convert a strftime-formatted span into a DateTime object.
        /// For the conversion, the current culture is used.
        /// </summary>
        /// <param name="input">The span which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="FormatException">There is something wrong with the provided input</exception>
        /// <exception cref="ArgumentOutOfRangeException">There is something wrong with the formatted date</exception>
        public static DateTime Parse(ReadOnlySpan<char> input, ReadOnlySpan<char> format) => 
            Parse(input, format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Convert a strftime-formatted span into a DateTime object.
        /// For the conversion, a specified culture is used.
        /// </summary>
        /// <param name="input">The span which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="culture">A culture-info used for the conversion</param>
        /// <param name="coherenceCheck">Throw exception on incoherence found</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="ArgumentNullException">culture is null</exception>
        /// <exception cref="FormatException">There is something wrong with the provided input</exception>
        /// <exception cref="ArgumentOutOfRangeException">There is something wrong with the formatted date</exception>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static DateTime Parse(ReadOnlySpan<char> input, ReadOnlySpan<char> format, CultureInfo culture, bool coherenceCheck = false)
        {
            if (culture == null) throw new ArgumentNullException(nameof(culture));

            var inputSpan = input;
            var formatter = Formatter.For(culture);
            var res = new DateTime(1900, 1, 1, 0, 0, 0);
            var now = DateTime.Now;
            var yearAssigned = false;
            var monthAssigned = false;
            var dayAssigned = false;
            var secondAssigned = false;
            var hourAssigned = false;
            var minuteAssigned = false;
            var dayOfWeekAssigned = false;
            string amPm = null;
            int? hour12 = null;
            var inputIndex = 0;
            var formatIndex = 0;

            while (inputIndex < input.Length && formatIndex < format.Length)
            {
                if (format[formatIndex] == '%' && formatIndex + 1 < format.Length)
                {
                    formatIndex++; // Move to specifier character
                    switch (format[formatIndex])
                    {
                        case '%':
                            inputIndex++;
                            break;
                        case 'Y':
                        {
                            var y = Formatter.ConsumeYearFull(ref inputSpan, ref inputIndex);
                            var yVal = Formatter.ParseYear(y);
                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yVal))
                                    throw new FormatException("%Y format incoherence");
                            }

                            res = res.AddYears(yVal - res.Year);
                            yearAssigned = true;
                            break;
                        }
                        case 'y':
                        {
                            var y = Formatter.ConsumeYearTwoDigits(ref inputSpan, ref inputIndex);
                            var intVal = Formatter.ParseYearTwoDigits(y);
                            var yVal = (now.Year / 100) * 100 + intVal;
                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yVal))
                                    throw new FormatException("%y format incoherence");
                            }

                            res = res.AddYears(yVal - res.Year);
                            yearAssigned = true;
                            break;
                        }
                        case 'S':
                        {
                            var s = Formatter.ConsumeSecond(ref inputSpan, ref inputIndex);
                            var sVal = Formatter.ParseSecond(s);
                            if (coherenceCheck)
                            {
                                if (secondAssigned && !sVal.Equals(res.Second))
                                    throw new FormatException("%S format incoherence");
                            }

                            res = res.AddSeconds(sVal - res.Second);
                            secondAssigned = true;
                            break;
                        }
                        case 't':
                            _ = Formatter.ConsumeTab(ref inputSpan, ref inputIndex);
                            break;
                        case 'T':
                        {
                            var isoTime = Formatter.ConsumeIsoTime(ref inputSpan, ref inputIndex);

                            var (h, m, s) = Formatter.ParseIsoTime(isoTime);

                            if (coherenceCheck)
                            {
                                if (secondAssigned && !res.Second.Equals(s))
                                    throw new FormatException("Incoherent second");
                                if (minuteAssigned && !res.Minute.Equals(m))
                                    throw new FormatException("Incoherent minute");
                                if (hourAssigned && !res.Hour.Equals(h))
                                    throw new FormatException("Incoherent hour");
                            }

                            res = res.AddHours(h - res.Hour);
                            res = res.AddMinutes(m - res.Minute);
                            res = res.AddSeconds(s - res.Second);


                            hourAssigned = true;
                            minuteAssigned = true;
                            secondAssigned = true;
                            break;
                        }
                        case 'u':
                        {
                            var isoWeekDay = Formatter.ConsumeIsoWeekDay(ref inputSpan, ref inputIndex);
                            var weekDay = FromIsoWeekDay(Formatter.ParseIsoWeekDay(isoWeekDay));

                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && !res.DayOfWeek.Equals(weekDay))
                                {
                                    throw new FormatException("Weekday incoherence");
                                }
                            }

                            res = formatter.ToDayOfWeek(res, weekDay);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'n':
                            _ = Formatter.ConsumeNewLine(ref inputSpan, ref inputIndex);
                            break;
                        case 'm':
                        {
                            var m = Formatter.ConsumeMonth(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMonth(m);
                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(mVal))
                                    throw new FormatException("Month incoherence");
                            }

                            res = res.AddMonths(mVal - res.Month);
                            monthAssigned = true;
                            break;
                        }
                        case 'M':
                        {
                            var m = Formatter.ConsumeMinute(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMinute(m);
                            if (coherenceCheck)
                            {
                                if (minuteAssigned && !res.Minute.Equals(mVal))
                                    throw new FormatException("Minute incoherence");
                            }

                            res = res.AddMinutes(mVal - res.Minute);
                            minuteAssigned = true;
                            break;
                        }
                        case 'I':
                        {
                            var s = Formatter.ConsumeHour12(ref inputSpan, ref inputIndex);
                            var v = Formatter.ParseHour12(s);

                            if (coherenceCheck)
                            {
                                if (hour12 != null && hour12 != v)
                                    throw new FormatException("Hour incoherence");

                                if (hourAssigned && !ConvertTo12HourFormat(res.Hour).Equals(hour12))
                                {
                                    throw new FormatException("Hour incoherence");
                                }
                            }


                            hour12 = v;

                            if (amPm != null)
                            {
                                res = res.AddHours(ConvertTo24HourFormat(hour12.Value, amPm) - res.Hour);
                            }

                            break;
                        }
                        case 'p':
                        {
                            var consumed = Formatter.ConsumeAmPmDesignation(ref inputSpan, ref inputIndex);
                            var v = Formatter.ParseAmPmDesignation(consumed);

                            if (coherenceCheck)
                            {
                                if (hourAssigned)
                                {
                                    switch (v)
                                    {
                                        case "PM":
                                            if (res.Hour is 0 or <= 11)
                                                throw new FormatException("Hour incoherence");
                                            break;
                                        case "AM":
                                            if (res.Hour > 11)
                                                throw new FormatException("Hour incoherence");
                                            break;
                                    }
                                }
                            }

                            if (hour12 != null)
                            {
                                res = res.AddHours(ConvertTo24HourFormat(hour12.Value, v) - res.Hour);
                                hourAssigned = true;
                            }

                            amPm = v;
                            break;
                        }
                        case 'H':
                        {
                            var s = Formatter.ConsumeHour24(ref inputSpan, ref inputIndex);
                            var v = Formatter.ParseHour24(s);

                            if (coherenceCheck)
                            {
                                if (hourAssigned && !res.Hour.Equals(v))
                                    throw new FormatException("Hour incoherence");
                            }

                            res = res.AddHours(v - res.Hour);
                            hourAssigned = true;
                            break;
                        }
                        case 'B':
                        {
                            var s = formatter.ConsumeFullMonth(ref inputSpan, ref inputIndex);
                            var v = formatter.ParseMonthFull(s);
                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(v))
                                    throw new FormatException("Month incoherence");
                            }

                            res = res.AddMonths(v - res.Month);
                            monthAssigned = true;
                            break;
                        }
                        case 'b':
                        {
                            var s = formatter.ConsumeAbbreviatedMonth(ref inputSpan, ref inputIndex);
                            var v = formatter.ParseMonthAbbreviated(s);
                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(v))
                                    throw new FormatException("Month incoherence");
                            }

                            res = res.AddMonths(v - res.Month);
                            monthAssigned = true;
                            break;
                        }
                        case 'c':
                        {
                            var abbreviatedDayOfWeek =
                                formatter.ConsumeAbbreviatedDayOfWeek(ref inputSpan, ref inputIndex);
                            var weekDayVal = formatter.ParseDayOfWeekAbbreviated(abbreviatedDayOfWeek);
                            inputIndex++;

                            var abbrMonth = formatter.ConsumeAbbreviatedMonth(ref inputSpan, ref inputIndex);
                            var abbrMonthVal = formatter.ParseMonthAbbreviated(abbrMonth);
                            inputIndex++;

                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var dayOfTheMonthVal = Formatter.ParseDayOfTheMonthFlexible(d);
                            inputIndex++;

                            var h = Formatter.ConsumeHour24(ref inputSpan, ref inputIndex);
                            var hourVal = Formatter.ParseHour24(h);
                            inputIndex++;

                            var m = Formatter.ConsumeMinute(ref inputSpan, ref inputIndex);
                            var minuteVal = Formatter.ParseMinute(m);
                            inputIndex++;

                            var s = Formatter.ConsumeSecond(ref inputSpan, ref inputIndex);
                            var secondVal = Formatter.ParseSecond(s);
                            inputIndex++;

                            var y = Formatter.ConsumeYearFull(ref inputSpan, ref inputIndex);
                            var yearVal = Formatter.ParseYear(y);

                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yearVal))
                                    throw new FormatException("Year incoherence");
                                if (monthAssigned && !res.Month.Equals(abbrMonthVal))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dayOfTheMonthVal))
                                    throw new FormatException("Day of the month incoherence");
                                if (hourAssigned && !res.Hour.Equals(hourVal))
                                    throw new FormatException("Hour incoherence");
                                if (minuteAssigned && !res.Minute.Equals(minuteVal))
                                    throw new FormatException("Minute incoherence");
                                if (secondAssigned && !res.Second.Equals(secondVal))
                                    throw new FormatException("Second incoherence");
                            }

                            res = res.AddYears(yearVal - res.Year);
                            res = res.AddMonths(abbrMonthVal - res.Month);
                            res = res.AddDays(dayOfTheMonthVal - res.Day);
                            res = res.AddHours(hourVal - res.Hour);
                            res = res.AddMinutes(minuteVal - res.Minute);
                            res = res.AddSeconds(secondVal - res.Second);

                            hourAssigned = true;
                            minuteAssigned = true;
                            secondAssigned = true;
                            dayAssigned = true;
                            dayOfWeekAssigned = true;
                            monthAssigned = true;
                            yearAssigned = true;

                            if (coherenceCheck)
                            {
                                if (res.DayOfWeek != weekDayVal)
                                    throw new FormatException("Day of week incoherence");
                            }

                            break;
                        }
                        case 'a':
                        {
                            var weekDay = formatter.ConsumeAbbreviatedDayOfWeek(ref inputSpan, ref inputIndex);
                            var weekDayVal = formatter.ParseDayOfWeekAbbreviated(weekDay);
                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && res.DayOfWeek != weekDayVal)
                                    throw new FormatException("Day of week incoherence");

                                if (dayAssigned && monthAssigned && yearAssigned && res.DayOfWeek != weekDayVal)
                                    throw new FormatException("Day of week incoherence");
                            }

                            res = formatter.ToDayOfWeek(res, weekDayVal);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'A':
                        {
                            var d = formatter.ConsumeDayOfWeek(ref inputSpan, ref inputIndex);
                            var v = formatter.ParseDayOfWeekFull(d);
                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && res.DayOfWeek != v)
                                    throw new FormatException("Day of week incoherence");

                                if (dayAssigned && monthAssigned && yearAssigned && res.DayOfWeek != v)
                                    throw new FormatException("Day of week incoherence");
                            }

                            res = formatter.ToDayOfWeek(res, v);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'd':
                        {
                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var v = Formatter.ParseDayOfTheMonthZeroPadded(d);
                            if (coherenceCheck)
                            {
                                if (dayAssigned && !res.Day.Equals(v))
                                    throw new FormatException("Day of the month incoherence");
                            }

                            res = res.AddDays(v - res.Day);
                            dayAssigned = true;
                            break;
                        }
                        case 'e':
                        {
                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var v = Formatter.ParseDayOfTheMonthSpacePadded(d);
                            if (coherenceCheck)
                            {
                                if (dayAssigned && !res.Day.Equals(v))
                                    throw new FormatException("Day of the month incoherence");
                            }

                            res = res.AddDays(v - res.Day);
                            dayAssigned = true;
                            break;
                        }
                        case 'D':
                        {
                            var m = Formatter.ConsumeMonth(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMonth(m);
                            inputIndex++;

                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var dVal = Formatter.ParseDayOfTheMonthFlexible(d);
                            inputIndex++;

                            var y = Formatter.ConsumeYearTwoDigits(ref inputSpan, ref inputIndex);
                            var intVal = Formatter.ParseYearTwoDigits(y);
                            var yVal = (now.Year / 100) * 100 + intVal;

                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(mVal))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dVal))
                                    throw new FormatException("Day incoherence");
                            }

                            res = res.AddMonths(mVal - res.Month);
                            res = res.AddDays(dVal - res.Day);
                            if (!yearAssigned)
                            {
                                res = res.AddYears(yVal - res.Year);
                            }

                            monthAssigned = true;
                            dayAssigned = true;
                            break;
                        }
                        case 'F':
                        {
                            var y = Formatter.ConsumeYearFull(ref inputSpan, ref inputIndex);
                            var yVal = Formatter.ParseYear(y);
                            inputIndex++;

                            var m = Formatter.ConsumeMonth(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMonth(m);
                            inputIndex++;

                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var dVal = Formatter.ParseDayOfTheMonthFlexible(d);

                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yVal))
                                    throw new FormatException("Year incoherence");
                                if (monthAssigned && !res.Month.Equals(mVal))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dVal))
                                    throw new FormatException("Day incoherence");
                            }

                            res = res.AddYears(yVal - res.Year);
                            res = res.AddMonths(mVal - res.Month);
                            res = res.AddDays(dVal - res.Day);

                            yearAssigned = true;
                            monthAssigned = true;
                            dayAssigned = true;
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'j':
                        {
                            var d = Formatter.ConsumeDayOfYear(inputSpan, ref inputIndex);
                            var intVal = Formatter.ParseDayOfYear(d);
                            var dVal = new DateTime(res.Year, 1, 1).AddDays(intVal - 1);

                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(dVal.Month))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dVal.Day))
                                    throw new FormatException("Day incoherence");
                            }

                            res = res.AddYears(dVal.Year - res.Year);
                            res = res.AddMonths(dVal.Month - res.Month);
                            res = res.AddDays(dVal.Day - res.Day);
                            monthAssigned = true;
                            dayAssigned = true;
                            break;
                        }
                        case 'w':
                        {
                            var d = Formatter.ConsumeWeekDaySundayBased(inputSpan, ref inputIndex);
                            var v = FromWeekDaySundayBased(Formatter.ParseWeekDaySundayBased(d));
                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && !res.DayOfWeek.Equals(v))
                                    throw new FormatException("Weekday incoherence");
                            }

                            res = formatter.ToDayOfWeek(res, v);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        default:
                            throw new FormatException($"Unrecognized format: %{format[formatIndex]}");
                    }
                }
                else
                {
                    // Literal character - skip in input
                    inputIndex++;
                }

                formatIndex++;
            }

            return res;
        }

        /// <summary>
        /// Convert a strftime-formatted string into a DateTime object.
        /// For the conversion, a specified culture is used.
        /// </summary>
        /// <param name="input">The string which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="culture">A culture-info used for the conversion</param>
        /// <param name="coherenceCheck">Throw exception on incoherence found</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="ArgumentNullException">input, format, or culture is null</exception>
        /// <exception cref="FormatException">There is something wrong with the provided input</exception>
        /// <exception cref="ArgumentOutOfRangeException">There is something wrong with the formatted date</exception>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static DateTime Parse(string input, string format, CultureInfo culture, bool coherenceCheck = false)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (culture == null) throw new ArgumentNullException(nameof(culture));

            var inputSpan = input.AsSpan();
            var formatter = Formatter.For(culture);
            var res = new DateTime(1900, 1, 1, 0, 0, 0);
            var now = DateTime.Now;
            var yearAssigned = false;
            var monthAssigned = false;
            var dayAssigned = false;
            var secondAssigned = false;
            var hourAssigned = false;
            var minuteAssigned = false;
            var dayOfWeekAssigned = false;
            string amPm = null;
            int? hour12 = null;
            var inputIndex = 0;
            var formatIndex = 0;

            while (inputIndex < input.Length && formatIndex < format.Length)
            {
                if (format[formatIndex] == '%' && formatIndex + 1 < format.Length)
                {
                    formatIndex++; // Move to specifier character
                    switch (format[formatIndex])
                    {
                        case '%':
                            inputIndex++;
                            break;
                        case 'Y':
                        {
                            var y = Formatter.ConsumeYearFull(ref inputSpan, ref inputIndex);
                            var yVal = Formatter.ParseYear(y);
                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yVal))
                                    throw new FormatException("%Y format incoherence");
                            }

                            res = res.AddYears(yVal - res.Year);
                            yearAssigned = true;
                            break;
                        }
                        case 'y':
                        {
                            var y = Formatter.ConsumeYearTwoDigits(ref inputSpan, ref inputIndex);
                            var intVal = Formatter.ParseYearTwoDigits(y);
                            var yVal = (now.Year / 100) * 100 + intVal;
                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yVal))
                                    throw new FormatException("%y format incoherence");
                            }

                            res = res.AddYears(yVal - res.Year);
                            yearAssigned = true;
                            break;
                        }
                        case 'S':
                        {
                            var s = Formatter.ConsumeSecond(ref inputSpan, ref inputIndex);
                            var sVal = Formatter.ParseSecond(s);
                            if (coherenceCheck)
                            {
                                if (secondAssigned && !sVal.Equals(res.Second))
                                    throw new FormatException("%S format incoherence");
                            }

                            res = res.AddSeconds(sVal - res.Second);
                            secondAssigned = true;
                            break;
                        }
                        case 't':
                            _ = Formatter.ConsumeTab(ref inputSpan, ref inputIndex);
                            break;
                        case 'T':
                        {
                            var isoTime = Formatter.ConsumeIsoTime(ref inputSpan, ref inputIndex);

                            var (h, m, s) = Formatter.ParseIsoTime(isoTime);

                            if (coherenceCheck)
                            {
                                if (secondAssigned && !res.Second.Equals(s))
                                    throw new FormatException("Incoherent second");
                                if (minuteAssigned && !res.Minute.Equals(m))
                                    throw new FormatException("Incoherent minute");
                                if (hourAssigned && !res.Hour.Equals(h))
                                    throw new FormatException("Incoherent hour");
                            }

                            res = res.AddHours(h - res.Hour);
                            res = res.AddMinutes(m - res.Minute);
                            res = res.AddSeconds(s - res.Second);


                            hourAssigned = true;
                            minuteAssigned = true;
                            secondAssigned = true;
                            break;
                        }
                        case 'u':
                        {
                            var isoWeekDay = Formatter.ConsumeIsoWeekDay(ref inputSpan, ref inputIndex);
                            var weekDay = FromIsoWeekDay(Formatter.ParseIsoWeekDay(isoWeekDay));

                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && !res.DayOfWeek.Equals(weekDay))
                                {
                                    throw new FormatException("Weekday incoherence");
                                }
                            }

                            res = formatter.ToDayOfWeek(res, weekDay);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'n':
                            _ = Formatter.ConsumeNewLine(ref inputSpan, ref inputIndex);
                            break;
                        case 'm':
                        {
                            var m = Formatter.ConsumeMonth(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMonth(m);
                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(mVal))
                                    throw new FormatException("Month incoherence");
                            }

                            res = res.AddMonths(mVal - res.Month);
                            monthAssigned = true;
                            break;
                        }
                        case 'M':
                        {
                            var m = Formatter.ConsumeMinute(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMinute(m);
                            if (coherenceCheck)
                            {
                                if (minuteAssigned && !res.Minute.Equals(mVal))
                                    throw new FormatException("Minute incoherence");
                            }

                            res = res.AddMinutes(mVal - res.Minute);
                            minuteAssigned = true;
                            break;
                        }
                        case 'I':
                        {
                            var s = Formatter.ConsumeHour12(ref inputSpan, ref inputIndex);
                            var v = Formatter.ParseHour12(s);

                            if (coherenceCheck)
                            {
                                if (hour12 != null && hour12 != v)
                                    throw new FormatException("Hour incoherence");

                                if (hourAssigned && !ConvertTo12HourFormat(res.Hour).Equals(hour12))
                                {
                                    throw new FormatException("Hour incoherence");
                                }
                            }


                            hour12 = v;

                            if (amPm != null)
                            {
                                res = res.AddHours(ConvertTo24HourFormat(hour12.Value, amPm) - res.Hour);
                            }

                            break;
                        }
                        case 'p':
                        {
                            var consumed = Formatter.ConsumeAmPmDesignation(ref inputSpan, ref inputIndex);
                            var v = Formatter.ParseAmPmDesignation(consumed);

                            if (coherenceCheck)
                            {
                                if (hourAssigned)
                                {
                                    switch (v)
                                    {
                                        case "PM":
                                            if (res.Hour is 0 or <= 11)
                                                throw new FormatException("Hour incoherence");
                                            break;
                                        case "AM":
                                            if (res.Hour > 11)
                                                throw new FormatException("Hour incoherence");
                                            break;
                                    }
                                }
                            }

                            if (hour12 != null)
                            {
                                res = res.AddHours(ConvertTo24HourFormat(hour12.Value, v) - res.Hour);
                                hourAssigned = true;
                            }

                            amPm = v;
                            break;
                        }
                        case 'H':
                        {
                            var s = Formatter.ConsumeHour24(ref inputSpan, ref inputIndex);
                            var v = Formatter.ParseHour24(s);

                            if (coherenceCheck)
                            {
                                if (hourAssigned && !res.Hour.Equals(v))
                                    throw new FormatException("Hour incoherence");
                            }

                            res = res.AddHours(v - res.Hour);
                            hourAssigned = true;
                            break;
                        }
                        case 'B':
                        {
                            var s = formatter.ConsumeFullMonth(ref inputSpan, ref inputIndex);
                            var v = formatter.ParseMonthFull(s);
                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(v))
                                    throw new FormatException("Month incoherence");
                            }

                            res = res.AddMonths(v - res.Month);
                            monthAssigned = true;
                            break;
                        }
                        case 'b':
                        {
                            var s = formatter.ConsumeAbbreviatedMonth(ref inputSpan, ref inputIndex);
                            var v = formatter.ParseMonthAbbreviated(s);
                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(v))
                                    throw new FormatException("Month incoherence");
                            }

                            res = res.AddMonths(v - res.Month);
                            monthAssigned = true;
                            break;
                        }
                        case 'c':
                        {
                            var abbreviatedDayOfWeek =
                                formatter.ConsumeAbbreviatedDayOfWeek(ref inputSpan, ref inputIndex);
                            var weekDayVal = formatter.ParseDayOfWeekAbbreviated(abbreviatedDayOfWeek);
                            inputIndex++;

                            var abbrMonth = formatter.ConsumeAbbreviatedMonth(ref inputSpan, ref inputIndex);
                            var abbrMonthVal = formatter.ParseMonthAbbreviated(abbrMonth);
                            inputIndex++;

                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var dayOfTheMonthVal = Formatter.ParseDayOfTheMonthFlexible(d);
                            inputIndex++;

                            var h = Formatter.ConsumeHour24(ref inputSpan, ref inputIndex);
                            var hourVal = Formatter.ParseHour24(h);
                            inputIndex++;

                            var m = Formatter.ConsumeMinute(ref inputSpan, ref inputIndex);
                            var minuteVal = Formatter.ParseMinute(m);
                            inputIndex++;

                            var s = Formatter.ConsumeSecond(ref inputSpan, ref inputIndex);
                            var secondVal = Formatter.ParseSecond(s);
                            inputIndex++;

                            var y = Formatter.ConsumeYearFull(ref inputSpan, ref inputIndex);
                            var yearVal = Formatter.ParseYear(y);

                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yearVal))
                                    throw new FormatException("Year incoherence");
                                if (monthAssigned && !res.Month.Equals(abbrMonthVal))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dayOfTheMonthVal))
                                    throw new FormatException("Day of the month incoherence");
                                if (hourAssigned && !res.Hour.Equals(hourVal))
                                    throw new FormatException("Hour incoherence");
                                if (minuteAssigned && !res.Minute.Equals(minuteVal))
                                    throw new FormatException("Minute incoherence");
                                if (secondAssigned && !res.Second.Equals(secondVal))
                                    throw new FormatException("Second incoherence");
                            }

                            res = res.AddYears(yearVal - res.Year);
                            res = res.AddMonths(abbrMonthVal - res.Month);
                            res = res.AddDays(dayOfTheMonthVal - res.Day);
                            res = res.AddHours(hourVal - res.Hour);
                            res = res.AddMinutes(minuteVal - res.Minute);
                            res = res.AddSeconds(secondVal - res.Second);

                            hourAssigned = true;
                            minuteAssigned = true;
                            secondAssigned = true;
                            dayAssigned = true;
                            dayOfWeekAssigned = true;
                            monthAssigned = true;
                            yearAssigned = true;

                            if (coherenceCheck)
                            {
                                if (res.DayOfWeek != weekDayVal)
                                    throw new FormatException("Day of week incoherence");
                            }

                            break;
                        }
                        case 'a':
                        {
                            var weekDay = formatter.ConsumeAbbreviatedDayOfWeek(ref inputSpan, ref inputIndex);
                            var weekDayVal = formatter.ParseDayOfWeekAbbreviated(weekDay);
                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && res.DayOfWeek != weekDayVal)
                                    throw new FormatException("Day of week incoherence");

                                if (dayAssigned && monthAssigned && yearAssigned && res.DayOfWeek != weekDayVal)
                                    throw new FormatException("Day of week incoherence");
                            }

                            res = formatter.ToDayOfWeek(res, weekDayVal);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'A':
                        {
                            var d = formatter.ConsumeDayOfWeek(ref inputSpan, ref inputIndex);
                            var v = formatter.ParseDayOfWeekFull(d);
                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && res.DayOfWeek != v)
                                    throw new FormatException("Day of week incoherence");

                                if (dayAssigned && monthAssigned && yearAssigned && res.DayOfWeek != v)
                                    throw new FormatException("Day of week incoherence");
                            }

                            res = formatter.ToDayOfWeek(res, v);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'd':
                        {
                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var v = Formatter.ParseDayOfTheMonthZeroPadded(d);
                            if (coherenceCheck)
                            {
                                if (dayAssigned && !res.Day.Equals(v))
                                    throw new FormatException("Day of the month incoherence");
                            }

                            res = res.AddDays(v - res.Day);
                            dayAssigned = true;
                            break;
                        }
                        case 'e':
                        {
                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var v = Formatter.ParseDayOfTheMonthSpacePadded(d);
                            if (coherenceCheck)
                            {
                                if (dayAssigned && !res.Day.Equals(v))
                                    throw new FormatException("Day of the month incoherence");
                            }

                            res = res.AddDays(v - res.Day);
                            dayAssigned = true;
                            break;
                        }
                        case 'D':
                        {
                            var m = Formatter.ConsumeMonth(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMonth(m);
                            inputIndex++;

                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var dVal = Formatter.ParseDayOfTheMonthFlexible(d);
                            inputIndex++;

                            var y = Formatter.ConsumeYearTwoDigits(ref inputSpan, ref inputIndex);
                            var intVal = Formatter.ParseYearTwoDigits(y);
                            var yVal = (now.Year / 100) * 100 + intVal;

                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(mVal))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dVal))
                                    throw new FormatException("Day incoherence");
                            }

                            res = res.AddMonths(mVal - res.Month);
                            res = res.AddDays(dVal - res.Day);
                            if (!yearAssigned)
                            {
                                res = res.AddYears(yVal - res.Year);
                            }

                            monthAssigned = true;
                            dayAssigned = true;
                            break;
                        }
                        case 'F':
                        {
                            var y = Formatter.ConsumeYearFull(ref inputSpan, ref inputIndex);
                            var yVal = Formatter.ParseYear(y);
                            inputIndex++;

                            var m = Formatter.ConsumeMonth(ref inputSpan, ref inputIndex);
                            var mVal = Formatter.ParseMonth(m);
                            inputIndex++;

                            var d = Formatter.ConsumeDayOfTheMonth(inputSpan, ref inputIndex);
                            var dVal = Formatter.ParseDayOfTheMonthFlexible(d);

                            if (coherenceCheck)
                            {
                                if (yearAssigned && !res.Year.Equals(yVal))
                                    throw new FormatException("Year incoherence");
                                if (monthAssigned && !res.Month.Equals(mVal))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dVal))
                                    throw new FormatException("Day incoherence");
                            }

                            res = res.AddYears(yVal - res.Year);
                            res = res.AddMonths(mVal - res.Month);
                            res = res.AddDays(dVal - res.Day);

                            yearAssigned = true;
                            monthAssigned = true;
                            dayAssigned = true;
                            dayOfWeekAssigned = true;
                            break;
                        }
                        case 'j':
                        {
                            var d = Formatter.ConsumeDayOfYear(inputSpan, ref inputIndex);
                            var intVal = Formatter.ParseDayOfYear(d);
                            var dVal = new DateTime(res.Year, 1, 1).AddDays(intVal - 1);

                            if (coherenceCheck)
                            {
                                if (monthAssigned && !res.Month.Equals(dVal.Month))
                                    throw new FormatException("Month incoherence");
                                if (dayAssigned && !res.Day.Equals(dVal.Day))
                                    throw new FormatException("Day incoherence");
                            }

                            res = res.AddYears(dVal.Year - res.Year);
                            res = res.AddMonths(dVal.Month - res.Month);
                            res = res.AddDays(dVal.Day - res.Day);
                            monthAssigned = true;
                            dayAssigned = true;
                            break;
                        }
                        case 'w':
                        {
                            var d = Formatter.ConsumeWeekDaySundayBased(inputSpan, ref inputIndex);
                            var v = FromWeekDaySundayBased(Formatter.ParseWeekDaySundayBased(d));
                            if (coherenceCheck)
                            {
                                if (dayOfWeekAssigned && !res.DayOfWeek.Equals(v))
                                    throw new FormatException("Weekday incoherence");
                            }

                            res = formatter.ToDayOfWeek(res, v);
                            dayOfWeekAssigned = true;
                            break;
                        }
                        default:
                            throw new FormatException($"Unrecognized format: %{format[formatIndex]}");
                    }
                }
                else
                {
                    // Literal character - skip in input
                    inputIndex++;
                }

                formatIndex++;
            }

            return res;
        }

        private static int ConvertTo12HourFormat(int hour24Hour)
        {
            return hour24Hour switch
            {
                //TODO: change exception message
                < 0 or > 23 => throw new ArgumentException("L'ora fornita non � valida."),
                0 => 12,
                <= 12 => hour24Hour,
                _ => hour24Hour - 12
            };
        }

        private static int ConvertTo24HourFormat(int hour12Hour, string amPm)
        {
            if (hour12Hour < 1 || hour12Hour > 12 || (amPm.ToUpper() != "AM" && amPm.ToUpper() != "PM"))
            {
                //TODO: change exception message
                throw new ArgumentException("L'ora fornita non � valida.");
            }

            if (hour12Hour == 12 && amPm.ToUpper() == "AM")
            {
                hour12Hour = 0;
            }

            if (hour12Hour != 12 && amPm.ToUpper() == "PM")
            {
                hour12Hour += 12;
            }

            return hour12Hour;
        }

        private static DayOfWeek FromIsoWeekDay(int isoWeekDay)
        {
            return isoWeekDay switch
            {
                1 => DayOfWeek.Monday,
                2 => DayOfWeek.Tuesday,
                3 => DayOfWeek.Wednesday,
                4 => DayOfWeek.Thursday,
                5 => DayOfWeek.Friday,
                6 => DayOfWeek.Saturday,
                7 => DayOfWeek.Sunday,
                _ => throw new ArgumentException()
            };
        }

        private static DayOfWeek FromWeekDaySundayBased(int weekDay)
        {
            return weekDay switch
            {
                0 => DayOfWeek.Sunday,
                1 => DayOfWeek.Monday,
                2 => DayOfWeek.Tuesday,
                3 => DayOfWeek.Wednesday,
                4 => DayOfWeek.Thursday,
                5 => DayOfWeek.Friday,
                6 => DayOfWeek.Saturday,
                _ => throw new ArgumentException()
            };
        }
    }
}