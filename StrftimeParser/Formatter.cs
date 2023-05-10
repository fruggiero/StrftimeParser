using System;
using System.Globalization;

namespace StrftimeParser
{
    public abstract class Formatter
    {
        public abstract string ConsumeDayOfWeek(ref string input, ref int inputIndex);
        public abstract string ConsumeAbbreviatedDayOfWeek(ref string input, ref int inputIndex);

        public abstract DayOfWeek ParseDayOfWeekAbbreviated(string input);
        
        public DateTime ToWeekDay(DayOfWeek dayOfWeek)
        {
            var now = DateTime.Now;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            while (now.DayOfWeek != firstDayOfWeek) now = now.AddDays(-1);
            while (now.DayOfWeek != dayOfWeek) now = now.AddDays(1);
            return now;
        }
    }
}