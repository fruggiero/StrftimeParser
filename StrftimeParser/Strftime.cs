using System;
using System.Globalization;

namespace StrftimeParser
{
    public static class Strftime
    {
        public static DateTime ToDateTime(string input, string format)
        {
            string abbrWeekDay = null;
            string fullWeekDay = null;
            switch (CultureInfo.CurrentCulture.Name)
            {
                
            }
            var formatter = new EnUsFormatter();
	
            for (int inputIndex = 0; inputIndex < input.Length; inputIndex++)
            {
                for (int formatIndex = 0; formatIndex < format.Length; formatIndex++)
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
                                        abbrWeekDay = formatter.ConsumeAbbreviatedDayOfWeek(ref input, ref inputIndex);
                                        break;
                                    case 'A':
                                        fullWeekDay = formatter.ConsumeDayOfWeek(ref input, ref inputIndex);
                                        break;
                                }
                            }
                            break;
                        default:
                            inputIndex++;
                            break;
                    }
                }
            }

            if (abbrWeekDay != null)
            {
                return formatter.ToWeekDay(formatter.ParseDayOfWeekAbbreviated(abbrWeekDay));
            }

            throw new FormatException();
        }
    }
}