using System;
using System.Globalization;
using DateTime = System.DateTime;

namespace StrftimeParser
{
    public static class Strftime
    {
        public static DateTime Parse(string input, string format)
        {
            bool setDate;
            string abbrWeekDay = null, fullWeekDay = null;
            string dayOfTheMonthZeroPadded = null, dayOfTheMonthSpacePadded = null;
            
            switch (CultureInfo.CurrentCulture.Name)
            {
                
            }

            var res = DateTime.Now;
            var formatter = new EnUsFormatter();
	
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
                                    case 'a':
                                    {
                                        var weekDay = formatter
                                            .ConsumeAbbreviatedDayOfWeek(ref input, ref inputIndex);
                                        if (abbrWeekDay != null && !weekDay.Equals(abbrWeekDay))
                                            throw new FormatException("Day of week is incoherent");
                                        abbrWeekDay = weekDay;
                                        break;
                                    }
                                    case 'A':
                                    {
                                        var weekDay = formatter.ConsumeDayOfWeek(ref input, ref inputIndex);
                                        if(fullWeekDay != null && !weekDay.Equals(fullWeekDay))
                                            throw new FormatException("Day of week is incoherent");
                                        fullWeekDay = weekDay;
                                        break;
                                    }
                                    case 'd':
                                    {
                                        var dayOfTheMonth = Formatter.ConsumeDayOfTheMonth(input, ref inputIndex);
                                        if (dayOfTheMonthZeroPadded != null &&
                                            !dayOfTheMonth.Equals(dayOfTheMonthZeroPadded))
                                            throw new FormatException("Day of the month is incoherent");
                                        dayOfTheMonthZeroPadded = dayOfTheMonth;
                                        break;
                                    }
                                    case 'e':
                                    {
                                        var dayOfTheMonth = Formatter.ConsumeDayOfTheMonth(input, ref inputIndex);
                                        if (dayOfTheMonthSpacePadded != null &&
                                            !dayOfTheMonth.Equals(dayOfTheMonthSpacePadded))
                                            throw new FormatException("Day of the month is incoherent");
                                        dayOfTheMonthSpacePadded = dayOfTheMonth;
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

            bool checkDay = false;
            
            // Day of the month
            if (dayOfTheMonthZeroPadded != null)
            {
                var dayOfTheMonth = int.Parse(dayOfTheMonthZeroPadded, CultureInfo.InvariantCulture);
                res = Formatter.ToDayOfTheMonth(res, dayOfTheMonth);
                if (res.Day != dayOfTheMonth)
                    throw new FormatException("Incoherent day of the month");
                checkDay = true;
            }

            if (dayOfTheMonthSpacePadded != null)
            {
                var dayOfTheMonth = int.Parse(dayOfTheMonthSpacePadded, CultureInfo.InvariantCulture);
                if(checkDay && res.Day != dayOfTheMonth)
                    throw new FormatException("Incoherent day of the month");
                res = Formatter.ToDayOfTheMonth(res, dayOfTheMonth);
                if (res.Day != dayOfTheMonth)
                    throw new FormatException("Incoherent day of the month");
                checkDay = true;
            }

            // Day of week
            if (abbrWeekDay != null)
            {
                var tmp = Formatter.ToDayOfWeek(res, formatter.ParseDayOfWeekAbbreviated(abbrWeekDay));
                if (checkDay && tmp.Day != res.Day) throw new FormatException("Incoherent day");
                res = tmp;
                checkDay = true;
            }

            if (fullWeekDay != null)
            {
                var fullWeekDayDt = Formatter.ToDayOfWeek(res, formatter.ParseDayOfWeekFull(fullWeekDay));
                if (checkDay && fullWeekDayDt.Day != res.Day) throw new FormatException("Incoherent day");
                res = fullWeekDayDt;
                checkDay = true;
            }

            return res;
        }
    }
}