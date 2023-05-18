using System;
using System.Globalization;
using DateTime = System.DateTime;

namespace StrftimeParser
{
    public static class Strftime
    {
        public static DateTime Parse(string input, string format)
        {
            Formatter formatter = CultureInfo.CurrentCulture.Name switch
            {
                _ => new EnUsFormatter()
            };
            
            var elements = ParseElements(input, format, formatter);
            
            var now = DateTime.Now;
            
            DayOfWeek? dayOfWeek = null;
            int? dayOfTheMonth = null;
            int? month = null;
            int? year = null;
            
            // Short MM/DD/YY date, equivalent to %m/%d/%y
            if (elements.ShortMmDdYy != null)
            {
                var shortMmDdYy = Formatter.ParseShortMmDdYy(elements.ShortMmDdYy);
                if(dayOfTheMonth != null && !dayOfTheMonth.Equals(shortMmDdYy.Dd))
                    throw new FormatException("Incoherent day of the month");
                if (month != null && !month.Equals(shortMmDdYy.Mm))
                    throw new FormatException("Incoherent month");
                if (year != null && !year.Equals(shortMmDdYy.Yy))
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
                if (year != null && !year.Equals(shortYyyyMmDd.Yyyy))
                    throw new FormatException("Incoherent year");
                
                dayOfTheMonth = shortYyyyMmDd.Dd;
                month = shortYyyyMmDd.Mm;
                year = shortYyyyMmDd.Yyyy;
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
                if(dayOfTheMonth != null && dayOfTheMonth != dayOfTheMonthSpacePadded)
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
            
            var res = new DateTime(year ?? now.Year, month ?? now.Month, dayOfTheMonth ?? now.Day);
            if (dayOfTheMonth != null && dayOfWeek != null && res.DayOfWeek != dayOfWeek)
            {
                throw new FormatException("Incoherent day of week");
            }

            if (dayOfTheMonth == null && dayOfWeek != null)
            {
                res = Formatter.ToDayOfWeek(res, dayOfWeek.Value);
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
                            if(formatIndex + 1 < format.Length)
                            {
                                formatIndex++;
						
                                switch (format[formatIndex])
                                {
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
                                    case 'c':
                                    {
                                        var weekDay = formatter.ConsumeAbbreviatedDayOfWeek(ref input, ref inputIndex);
                                        if (res.AbbrWeekDay != null && !weekDay.Equals(res.AbbrWeekDay))
                                            throw new FormatException("%c format incoherence");
                                        res.AbbrWeekDay = weekDay;
                                        break;
                                        
                                        //TODO: finish full date parsing
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
                                        if(res.FullWeekDay != null && !weekDay.Equals(res.FullWeekDay))
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
    }
}