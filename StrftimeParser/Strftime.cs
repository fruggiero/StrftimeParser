using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using StrftimeParser.Formatters;
using DateTime = System.DateTime;

namespace StrftimeParser
{
    public static class Strftime
    {
        /// <summary>
        /// Convert a strftime-formatted string into a DateTime object.
        /// For the conversion, the current culture is used.
        /// This method is the equivalent of strptime in C
        /// </summary>
        /// <param name="input">The string which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="FormatException">There is something wrong with the formatted date</exception>
        /// <exception cref="ArgumentOutOfRangeException">There is something wrong with the formatted date</exception>
        public static DateTime Parse(string input, string format) => Parse(input, format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Convert a strftime-formatted string into a DateTime object.
        /// For the conversion, a specified culture is used.
        /// </summary>
        /// <param name="input">The string which contains the formatted date</param>
        /// <param name="format">A format specifier</param>
        /// <param name="culture">A culture-info used for the conversion</param>
        /// <returns>A DateTime object</returns>
        /// <exception cref="FormatException">There is something wrong with the formatted date</exception>
        /// <exception cref="ArgumentOutOfRangeException">There is something wrong with the formatted date</exception>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static DateTime Parse(string input, string format, CultureInfo culture)
        {
            Formatter formatter = culture.Name switch
            {
                "en-US" => new EnUsFormatter(),
                _ => new GenericFormatter(culture)
            };

            var elements = ParseElements(input, format, formatter);

            var now = DateTime.Now;

            DayOfWeek? dayOfWeek = null;
            int? dayOfTheMonth = null;
            int? month = null;
            var year = new WrappedInt(1970);
            int? hour = null;
            int? minute = null;
            int? second = null;
            string amPmDesignation = null;

            // Short MM/DD/YY date, equivalent to %m/%d/%y
            if (elements.ShortMmDdYy != null)
            {
                var shortMmDdYy = Formatter.ParseShortMmDdYy(elements.ShortMmDdYy);
                if (dayOfTheMonth != null && !dayOfTheMonth.Equals(shortMmDdYy.Dd))
                    throw new FormatException("Incoherent day of the month");
                if (month != null && !month.Equals(shortMmDdYy.Mm))
                    throw new FormatException("Incoherent month");
                if (!year.IsDefault && !year.Equals(shortMmDdYy.Yy))
                    throw new FormatException("Incoherent year");

                dayOfTheMonth = shortMmDdYy.Dd;
                month = shortMmDdYy.Mm;
                year = int.Parse(string.Concat(now.ToString("yyyy").Substring(0, 2),
                    shortMmDdYy.Yy.ToString().PadLeft(2, '0')));
            }

            // Short YYYY-MM-DD date, equivalent to %Y-%m-%d
            if (elements.ShortYyyyMmDd != null)
            {
                var shortYyyyMmDd = Formatter.ParseShortYyyyMmDd(elements.ShortYyyyMmDd);
                if (dayOfTheMonth != null && !dayOfTheMonth.Equals(shortYyyyMmDd.Dd))
                    throw new FormatException("Incoherent day of the month");
                if (month != null && !month.Equals(shortYyyyMmDd.Mm))
                    throw new FormatException("Incoherent month");
                if (!year.IsDefault && !year.Equals(shortYyyyMmDd.Yyyy))
                    throw new FormatException("Incoherent year");

                dayOfTheMonth = shortYyyyMmDd.Dd;
                month = shortYyyyMmDd.Mm;
                year = shortYyyyMmDd.Yyyy;
            }

            // Year
            if (elements.Year != null)
            {
                var y = Formatter.ParseYear(elements.Year);
                if (!year.IsDefault && !year.Equals(y))
                    throw new FormatException("Year incoherence");

                year = y;
            }

            // Year two digits
            int? yearTwoDigits = null;
            if (elements.YearTwoDigits != null)
            {
                yearTwoDigits = Formatter.ParseYearTwoDigits(elements.YearTwoDigits);
                if (!year.IsDefault && !yearTwoDigits.Equals(year % 100))
                    throw new FormatException("Year incoherence");
            }

            // Year divided by 100
            if (elements.YearDividedBy100 != null)
            {
                int yearCentury = Formatter.ParseYearDividedBy100(elements.YearDividedBy100);
                if(!year.IsDefault && !yearCentury.Equals(year / 100)) throw new FormatException("Incoherent year");

                if (year.IsDefault && yearTwoDigits != null)
                {
                    year = int.Parse($"{yearCentury}{yearTwoDigits}");
                }
            }

            // Day of the month
            if (elements.DayOfTheMonthZeroPadded != null)
            {
                var dayOfTheMonthZeroPadded = int.Parse(elements.DayOfTheMonthZeroPadded, CultureInfo.InvariantCulture);
                if (dayOfTheMonth != null && dayOfTheMonth != dayOfTheMonthZeroPadded)
                {
                    throw new FormatException("Incoherent day of the month");
                }

                dayOfTheMonth = dayOfTheMonthZeroPadded;
            }

            if (elements.DayOfTheMonthSpacePadded != null)
            {
                var dayOfTheMonthSpacePadded = int.Parse(elements.DayOfTheMonthSpacePadded, CultureInfo.InvariantCulture);
                if (dayOfTheMonth != null && dayOfTheMonth != dayOfTheMonthSpacePadded)
                {
                    throw new FormatException("Incoherent day of the month");
                }

                dayOfTheMonth = dayOfTheMonthSpacePadded;
            }

            // Day of week
            if (elements.AbbrWeekDay != null)
            {
                var abbrDayOfWeek = formatter.ParseDayOfWeekAbbreviated(elements.AbbrWeekDay);
                if (dayOfWeek != null && dayOfWeek != abbrDayOfWeek) throw new FormatException("Incoherent day");
                dayOfWeek = abbrDayOfWeek;
            }

            if (elements.FullWeekDay != null)
            {
                var fullWeekDay = formatter.ParseDayOfWeekFull(elements.FullWeekDay);
                if (dayOfWeek != null && dayOfWeek != fullWeekDay) throw new FormatException("Incoherent day");
                dayOfWeek = fullWeekDay;
            }

            // Month
            if (elements.MonthFull != null)
            {
                var monthFull = formatter.ParseMonthFull(elements.MonthFull);
                if (month != null && month != monthFull)
                {
                    throw new FormatException("Incoherent month");
                }
                month = monthFull;
            }

            if (elements.AbbreviatedMonth != null)
            {
                var abbrMonth = formatter.ParseMonthAbbreviated(elements.AbbreviatedMonth);
                if (month != null && month != abbrMonth)
                {
                    throw new FormatException("Incoherent month");
                }

                month = abbrMonth;
            }

            if (elements.Month != null)
            {
                var monthDecimal = Formatter.ParseMonth(elements.Month);
                if (month != null && !month.Equals(monthDecimal))
                {
                    throw new FormatException("Incoherent month");
                }

                month = monthDecimal;
            }

            // Hour 24
            if (elements.Hour24 != null)
            {
                var h24 = Formatter.ParseHour24(elements.Hour24);
                if (hour != null && hour != h24)
                    throw new FormatException("Incoherent hour");

                hour = h24;
            }

            // Hour 12
            if (elements.AmPm != null)
            {
                var amPm = Formatter.ParseAmPmDesignation(elements.AmPm);
                if (amPmDesignation != null && amPmDesignation != amPm)
                {
                    throw new FormatException("Incoherent AM/PM designation");
                }

                amPmDesignation = amPm;
            }

            if (elements.Hour12 != null)
            {
                if (amPmDesignation == null)
                    throw new FormatException("Found 12 hour format without AM/PM designation");

                var h12 = Formatter.ParseHour12(elements.Hour12);

                h12 = amPmDesignation switch
                {
                    "AM" => h12 == 12 ? 0 : h12,
                    "PM" => (h12 % 12) + 12,
                    _ => throw new ArgumentOutOfRangeException(amPmDesignation)
                };

                if (hour != null && !hour.Equals(h12))
                {
                    throw new FormatException("Incoherent hour");
                }

                hour = h12;
            }

            // Minute
            if (elements.Minute != null)
            {
                var m = Formatter.ParseMinute(elements.Minute);
                if (minute != null && !minute.Equals(m))
                    throw new FormatException("Incoherent minute");
                minute = m;
            }

            // Second
            if (elements.Second != null)
            {
                var s = Formatter.ParseSecond(elements.Second);
                if (second != null && !second.Equals(s))
                    throw new FormatException("Incoherent second");

                second = s;
            }

            // Iso time
            if (elements.IsoTime != null)
            {
                var (h, m, s) = Formatter.ParseIsoTime(elements.IsoTime);
                if (hour != null && !hour.Equals(h))
                    throw new FormatException("Incoherent hour");
                if (minute != null && !minute.Equals(m))
                    throw new FormatException("Incoherent minute");
                if (second != null && !second.Equals(s))
                    throw new FormatException("Incoherent second");

                hour = h;
                minute = m;
                second = s;
            }

            // Day of year
            if (elements.DayOfYear != null)
            {
                var dayOfYear = Formatter.ParseDayOfYear(elements.DayOfYear);
                var dtCalc = now.AddDays(dayOfYear - now.DayOfYear);
                if (dayOfTheMonth != null && !dayOfTheMonth.Equals(dtCalc.Day))
                    throw new FormatException("Incoherent day of the month with day of year");
                if (month != null && !month.Equals(dtCalc.Month))
                    throw new FormatException("Incoherent month with day of year");
                if (year != null && !year.Equals(dtCalc.Year))
                    throw new FormatException("Incoherent year with day of year");
                month = dtCalc.Month;
                dayOfTheMonth = dtCalc.Day;
                year = dtCalc.Year;
            }

            // Iso weekday
            if (elements.IsoWeekDay != null)
            {
                var weekDay = Formatter.ParseIsoWeekDay(elements.IsoWeekDay);
                if (dayOfWeek != null && FromIsoWeekDay(weekDay) != dayOfWeek)
                {
                    throw new FormatException("Weekday incoherence");
                }

                dayOfWeek = FromIsoWeekDay(weekDay);
            }

            // Weekday as a decimal number with Sunday as 0 (0-6)
            if (elements.WeekDaySundayBased != null)
            {
                var weekDay = Formatter.ParseWeekDaySundayBased(elements.WeekDaySundayBased);
                if (dayOfWeek != null && FromWeekDaySundayBased(weekDay) != dayOfWeek)
                    throw new FormatException("Weekday incoherence");

                dayOfWeek = FromWeekDaySundayBased(weekDay);
            }

            DateTime res;
            if (second is >= 60)
            {
                res = new DateTime(year ?? now.Year, month ?? now.Month, dayOfTheMonth ?? now.Day, hour ?? 0,
                    minute ?? 0, 59);

                res = res.AddSeconds(second.Value - 59);
            }
            else
            {
                res = new DateTime(year ?? now.Year, month ?? now.Month, dayOfTheMonth ?? now.Day, hour ?? 0,
                    minute ?? 0, second ?? 0);
            }

            if (dayOfTheMonth != null && dayOfWeek != null && res.DayOfWeek != dayOfWeek)
            {
                throw new FormatException("Incoherent day of week");
            }

            if (dayOfTheMonth == null && dayOfWeek != null)
            {
                res = formatter.ToDayOfWeek(res, dayOfWeek.Value);
            }

            return res;
        }

        private static ElementContainer ParseElements(string input, string format, Formatter formatter)
        {
            var res = new ElementContainer();

            for (var inputIndex = 0; inputIndex < input.Length; inputIndex++)
            {
                for (var formatIndex = 0; formatIndex < format.Length; formatIndex++)
                {
                    switch (format[formatIndex])
                    {
                        case '%':
                            if (formatIndex + 1 < format.Length)
                            {
                                formatIndex++;

                                switch (format[formatIndex])
                                {
                                    case 'Y':
                                        {
                                            var y = Formatter.ConsumeYearFull(ref input, ref inputIndex);
                                            if (res.Year != null && !res.Year.Equals(y))
                                                throw new FormatException("%Y format incoherence");

                                            res.Year = y;
                                            break;
                                        }
                                    case 'y':
                                        {
                                            var year = Formatter.ConsumeYearTwoDigits(ref input, ref inputIndex);
                                            if (res.YearTwoDigits != null && !res.YearTwoDigits.Equals(year))
                                                throw new FormatException("%y format incoherence");

                                            res.YearTwoDigits = year;
                                            break;
                                        }
                                    case 'S':
                                        {
                                            var second = Formatter.ConsumeSecond(ref input, ref inputIndex);
                                            if (res.Second != null && !res.Second.Equals(second))
                                                throw new FormatException("%S format incoherence");
                                            res.Second = second;
                                            break;
                                        }
                                    case 't':
                                        _ = Formatter.ConsumeTab(ref input, ref inputIndex);
                                        break;
                                    case 'T':
                                        var isoTime = Formatter.ConsumeIsoTime(ref input, ref inputIndex);
                                        if (res.IsoTime != null && !res.IsoTime.Equals(isoTime))
                                            throw new FormatException("%T format incoherence");
                                        res.IsoTime = isoTime;
                                        break;
                                    case 'u':
                                        var isoWeekDay = Formatter.ConsumeIsoWeekDay(ref input, ref inputIndex);
                                        if (res.IsoWeekDay != null && !res.IsoWeekDay.Equals(isoWeekDay))
                                            throw new FormatException("%u format incoherence");
                                        res.IsoWeekDay = isoWeekDay;
                                        break;
                                    case 'n':
                                        _ = Formatter.ConsumeNewLine(ref input, ref inputIndex);
                                        break;
                                    case 'm':
                                        {
                                            var month = Formatter.ConsumeMonth(ref input, ref inputIndex);
                                            if (res.Month != null && !res.Month.Equals(month))
                                                throw new FormatException("%m format incoherence");
                                            res.Month = month;
                                        }
                                        break;
                                    case 'M':
                                        {
                                            var minute = Formatter.ConsumeMinute(ref input, ref inputIndex);
                                            if (res.Minute != null && !res.Minute.Equals(minute))
                                                throw new FormatException("%M format incoherence");
                                            res.Minute = minute;
                                            break;
                                        }
                                    case 'I':
                                        {
                                            var consumed = Formatter.ConsumeHour12(ref input, ref inputIndex);
                                            if (res.Hour12 != null && !res.Hour12.Equals(consumed))
                                                throw new FormatException("%I format incoherence");
                                            res.Hour12 = consumed;
                                            break;
                                        }
                                    case 'p':
                                        {
                                            var consumed = Formatter.ConsumeAmPmDesignation(ref input, ref inputIndex);
                                            if (res.AmPm != null && !res.AmPm.Equals(consumed))
                                                throw new FormatException("%p format incoherence");
                                            res.AmPm = consumed;
                                            break;
                                        }
                                    case 'H':
                                        {
                                            var consumed = Formatter.ConsumeHour24(ref input, ref inputIndex);
                                            if (res.Hour24 != null && !res.Hour24.Equals(consumed))
                                                throw new FormatException("%H format incoherence");
                                            res.Hour24 = consumed;
                                            break;
                                        }
                                    case 'B':
                                        {
                                            var consumed = formatter.ConsumeFullMonth(ref input, ref inputIndex);
                                            if (res.MonthFull != null && !res.MonthFull.Equals(consumed))
                                                throw new FormatException("%B format incoherence");
                                            res.MonthFull = consumed;
                                            break;
                                        }
                                    case 'b':
                                        {
                                            var consumed = formatter.ConsumeAbbreviatedMonth(ref input, ref inputIndex);
                                            if (res.AbbreviatedMonth != null && !res.AbbreviatedMonth.Equals(consumed))
                                                throw new FormatException("%b format incoherence");
                                            res.AbbreviatedMonth = consumed;
                                            break;
                                        }
                                    case 'C':
                                        {
                                            var consumed = Formatter.ConsumeYearDividedBy100(ref input, ref inputIndex);
                                            if (res.YearDividedBy100 != null && !res.YearDividedBy100.Equals(consumed))
                                                throw new FormatException("%C format incoherence");
                                            res.YearDividedBy100 = consumed;
                                            break;
                                        }
                                    case 'c':
                                        {
                                            var weekDay = formatter.ConsumeAbbreviatedDayOfWeek(ref input, ref inputIndex);
                                            if (res.AbbrWeekDay != null && !weekDay.Equals(res.AbbrWeekDay))
                                                throw new FormatException("%c format incoherence");
                                            res.AbbrWeekDay = weekDay;
                                            inputIndex++;

                                            var month = formatter.ConsumeAbbreviatedMonth(ref input, ref inputIndex);
                                            if (res.AbbreviatedMonth != null && !res.AbbreviatedMonth.Equals(month))
                                                throw new FormatException("%c format incoherence");
                                            res.AbbreviatedMonth = month;
                                            inputIndex++;

                                            var dayOfTheMonth = Formatter.ConsumeDayOfTheMonth(input, ref inputIndex);
                                            if (res.DayOfTheMonthZeroPadded != null &&
                                                !dayOfTheMonth.Equals(res.DayOfTheMonthZeroPadded))
                                                throw new FormatException("%c format incoherence");
                                            res.DayOfTheMonthZeroPadded = dayOfTheMonth;
                                            inputIndex++;

                                            var hour = Formatter.ConsumeHour24(ref input, ref inputIndex);
                                            if (res.Hour24 != null && !res.Hour24.Equals(hour))
                                                throw new FormatException("%c format incoherence");
                                            res.Hour24 = hour;
                                            inputIndex++;

                                            var minute = Formatter.ConsumeMinute(ref input, ref inputIndex);
                                            if (res.Minute != null && !res.Minute.Equals(minute))
                                                throw new FormatException("%c format incoherence");
                                            res.Minute = minute;
                                            inputIndex++;

                                            var second = Formatter.ConsumeSecond(ref input, ref inputIndex);
                                            if (res.Second != null && !res.Second.Equals(second))
                                                throw new FormatException("%c format incoherence");
                                            res.Second = second;
                                            inputIndex++;

                                            var year = Formatter.ConsumeYearFull(ref input, ref inputIndex);
                                            if (res.Year != null && !res.Year.Equals(year))
                                                throw new FormatException("%c format incoherence");
                                            res.Year = year;

                                            break;
                                        }
                                    case 'a':
                                        {
                                            var weekDay = formatter
                                                .ConsumeAbbreviatedDayOfWeek(ref input, ref inputIndex);
                                            if (res.AbbrWeekDay != null && !weekDay.Equals(res.AbbrWeekDay))
                                                throw new FormatException("%a format incoherence");
                                            res.AbbrWeekDay = weekDay;
                                            break;
                                        }
                                    case 'A':
                                        {
                                            var weekDay = formatter.ConsumeDayOfWeek(ref input, ref inputIndex);
                                            if (res.FullWeekDay != null && !weekDay.Equals(res.FullWeekDay))
                                                throw new FormatException("%A format incoherence");
                                            res.FullWeekDay = weekDay;
                                            break;
                                        }
                                    case 'd':
                                        {
                                            var dayOfTheMonth = Formatter.ConsumeDayOfTheMonth(input, ref inputIndex);
                                            if (res.DayOfTheMonthZeroPadded != null &&
                                                !dayOfTheMonth.Equals(res.DayOfTheMonthZeroPadded))
                                                throw new FormatException("%d format incoherence");
                                            res.DayOfTheMonthZeroPadded = dayOfTheMonth;
                                            break;
                                        }
                                    case 'e':
                                        {
                                            var dayOfTheMonth = Formatter.ConsumeDayOfTheMonth(input, ref inputIndex);
                                            if (res.DayOfTheMonthSpacePadded != null &&
                                                !dayOfTheMonth.Equals(res.DayOfTheMonthSpacePadded))
                                                throw new FormatException("%e format incoherence");
                                            res.DayOfTheMonthSpacePadded = dayOfTheMonth;
                                            break;
                                        }
                                    case 'D':
                                        {
                                            var shortMmDdYy = Formatter.ConsumeShortMmDdYy(input, ref inputIndex);
                                            if (res.ShortMmDdYy != null && !res.ShortMmDdYy.Equals(shortMmDdYy))
                                                throw new FormatException("%D format incoherence");
                                            res.ShortMmDdYy = shortMmDdYy;
                                            break;
                                        }
                                    case 'F':
                                        {
                                            var shortYyyyMmDd = Formatter.ConsumeShortYyyyMmDd(input, ref inputIndex);
                                            if (res.ShortYyyyMmDd != null && !res.ShortYyyyMmDd.Equals(shortYyyyMmDd))
                                                throw new FormatException("%F format incoherence");
                                            res.ShortYyyyMmDd = shortYyyyMmDd;
                                            break;
                                        }
                                    case 'j':
                                        var dayOfYear = Formatter.ConsumeDayOfYear(input, ref inputIndex);
                                        if (res.DayOfYear != null && !res.DayOfYear.Equals(dayOfYear))
                                            throw new FormatException("%j format incoherence");
                                        res.DayOfYear = dayOfYear;
                                        break;
                                    case 'w':
                                        var weekDaySundayBased =
                                            Formatter.ConsumeWeekDaySundayBased(input, ref inputIndex);
                                        if (res.WeekDaySundayBased != null &&
                                            !res.WeekDaySundayBased.Equals(weekDaySundayBased))
                                            throw new FormatException("%w format incoherence");

                                        res.WeekDaySundayBased = weekDaySundayBased;
                                        break;
                                    default:
                                        throw new FormatException($"Unrecognized format: %{format[formatIndex]}");
                                }
                            }
                            break;
                        default:
                            inputIndex++;
                            break;
                    }
                }
            }

            return res;
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