using System;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    public class ItIttest
    {
        private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("it-IT");
        
        [Theory]
        [InlineData("lun", "%a", DayOfWeek.Monday)]
        [InlineData("mar", "%a", DayOfWeek.Tuesday)]
        [InlineData("mer", "%a", DayOfWeek.Wednesday)]
        [InlineData("gio", "%a", DayOfWeek.Thursday)]
        [InlineData("ven", "%a", DayOfWeek.Friday)]
        [InlineData("sab", "%a", DayOfWeek.Saturday)]
        [InlineData("dom", "%a", DayOfWeek.Sunday)]
        public void Parse_abbreviated_dayofweek(string input, string format, DayOfWeek dayOfWeek)
        {
            // arrange
            var now = DateTime.Now;
            var firstDayOfWeek = _culture.DateTimeFormat.FirstDayOfWeek;
            while (now.DayOfWeek != firstDayOfWeek) now = now.AddDays(-1);
            while (now.DayOfWeek != dayOfWeek) now = now.AddDays(1);
            
            // act
            var dt = Strftime.Parse(input, format, _culture);
            
            // assert
            dt.DayOfWeek.Should().Be(dayOfWeek);
        }

        [Theory]
        [InlineData("lunedì", "%A", DayOfWeek.Monday)]
        [InlineData("martedì", "%A", DayOfWeek.Tuesday)]
        [InlineData("mercoledì", "%A", DayOfWeek.Wednesday)]
        [InlineData("giovedì", "%A", DayOfWeek.Thursday)]
        [InlineData("venerdì", "%A", DayOfWeek.Friday)]
        [InlineData("sabato", "%A", DayOfWeek.Saturday)]
        [InlineData("domenica", "%A", DayOfWeek.Sunday)]
        public void Parse_DayOfWeek(string input, string format, DayOfWeek dayOfWeek)
        {
            // arrange
            var now = DateTime.Now;
            var firstDayOfWeek = _culture.DateTimeFormat.FirstDayOfWeek;
            while (now.DayOfWeek != firstDayOfWeek) now = now.AddDays(-1);
            while (now.DayOfWeek != dayOfWeek) now = now.AddDays(1);
            
            // act
            var dt = Strftime.Parse(input, format, _culture);
        
            // assert
            dt.DayOfWeek.Should().Be(dayOfWeek);
        }

        [Theory]
        [InlineData("gen", "%b", 1)]
        [InlineData("feb", "%b", 2)]
        [InlineData("mar", "%b", 3)]
        [InlineData("apr", "%b", 4)]
        [InlineData("mag", "%b", 5)]
        [InlineData("giu", "%b", 6)]
        [InlineData("lug", "%b", 7)]
        [InlineData("ago", "%b", 8)]
        [InlineData("set", "%b",9)]
        [InlineData("ott", "%b", 10)]
        [InlineData("nov", "%b", 11)]
        [InlineData("dic", "%b", 12)]
        public void Parse_Abbr_Month(string input, string format, int month)
        {
            // arrange
            var now = DateTime.Now;
            while (now.Month != 1) now = now.AddMonths(-1);
            while (now.Month != month) now = now.AddMonths(1);

            // act
            var dt = Strftime.Parse(input, format, _culture);

            // assert
            dt.Should().HaveMonth(month);
        }

        [Theory]
        [InlineData("gennaio", "%B", 1)]
        [InlineData("febbraio", "%B", 2)]
        [InlineData("marzo", "%B", 3)]
        [InlineData("aprile", "%B", 4)]
        [InlineData("maggio", "%B", 5)]
        [InlineData("giugno", "%B", 6)]
        [InlineData("luglio", "%B", 7)]
        [InlineData("agosto", "%B", 8)]
        [InlineData("settembre", "%B",9)]
        [InlineData("ottobre", "%B", 10)]
        [InlineData("novembre", "%B", 11)]
        [InlineData("dicembre", "%B", 12)]
        public void Parse_Month_Name(string input, string format, int month)
        {
            // arrange
            var now = DateTime.Now;
            while (now.Month != 1) now = now.AddMonths(-1);
            while (now.Month != month) now = now.AddMonths(1);

            // act
            var dt = Strftime.Parse(input, format, _culture);

            // assert
            dt.Should().HaveMonth(month);
        }
    }
}