using System.Globalization;

namespace StrftimeParser
{
    internal class GenericFormatter : Formatter
    {
        protected override CultureInfo Culture { get; }

        public GenericFormatter(CultureInfo culture)
        {
            Culture = culture;
        }
    }
}