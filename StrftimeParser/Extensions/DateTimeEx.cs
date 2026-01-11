using System;
using System.Globalization;

namespace StrftimeParser
{
    public static class DateTimeEx
    {
        public static string ToStrftimeString(this DateTime dt, string format, CultureInfo culture) => Strftime.ToString(dt, format, culture);

        public static string ToStrftimeString(this DateTime dt, string format) => Strftime.ToString(dt, format);
    }
}