using System;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    public class ParserTest
    {
        [Theory]
        [InlineData("Mon", DayOfWeek.Monday)]
        [InlineData("Tue", DayOfWeek.Tuesday)]
        [InlineData("Wed", DayOfWeek.Wednesday)]
        [InlineData("Thu", DayOfWeek.Thursday)]
        [InlineData("Fri", DayOfWeek.Friday)]
        [InlineData("Sat", DayOfWeek.Saturday)]
        [InlineData("Sun", DayOfWeek.Sunday)]
        public void Should_Return_DayOfWeek(string input, DayOfWeek dayOfWeek)
        {
            // arrange
            var format = "%a";
            var now = DateTime.Now;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            while (now.DayOfWeek != firstDayOfWeek) now = now.AddDays(-1);
            while (now.DayOfWeek != dayOfWeek) now = now.AddDays(1);
            
            // act
            var dt = Strftime.ToDateTime(input, format);
        
            // assert
            dt.Should().HaveDay(now.Day);
            dt.Should().HaveMonth(now.Month);
            dt.Should().HaveYear(now.Year);
        }
    }
}