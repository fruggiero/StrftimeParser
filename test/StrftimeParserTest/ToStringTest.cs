using System;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    public class ToStringTest
    {
        private readonly CultureInfo _culture = new("en-US");

        [Theory]
        [InlineData("%Y", "1970")]
        [InlineData("%y", "70")]
        [InlineData("%S", "05")]
        [InlineData("%t", "\t")]
        [InlineData("%T", "03:04:05")]
        [InlineData("%u", "5")]
        [InlineData("%n", "\n")]
        [InlineData("%m", "01")]
        [InlineData("%M", "04")]
        [InlineData("%I", "03")]
        [InlineData("%p", "AM")]
        [InlineData("%H", "03")]
        [InlineData("%B", "January")]
        [InlineData("%b", "Jan")]
        [InlineData("%C", "19")]
        [InlineData("%c", "Fri Jan 02 03:04:05 1970")]
        [InlineData("%a", "Fri")]
        [InlineData("%A", "Friday")]
        [InlineData("%d", "02")]
        [InlineData("%e", " 2")]
        [InlineData("%D", "01/02/70")]
        [InlineData("%F", "1970-01-02")]
        [InlineData("%j", "002")]
        [InlineData("%w", "5")]
        public void Should_ConvertToString(string format, string expectedResult)
        {
            var dt = new DateTime(1970, 1, 2, 3, 4, 5);

            var res = Strftime.ToString(dt, format, _culture);

            res.Should().Be(expectedResult);
        }
    }
}