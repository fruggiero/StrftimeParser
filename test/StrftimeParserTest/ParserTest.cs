using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    public class ParserTest
    {
        [SuppressMessage("ReSharper", "SimplifyStringInterpolation")]
        public static IEnumerable<object[]> ShouldThrow_WhenIncoherentDayOfMonth_WithDayOfWeek_Data()
        {
            var now = DateTime.Now;
            yield return new object[]
                { $"{now.Day.ToString().PadLeft(2, '0')} {now.AddDays(1).DayOfWeek.ToString()[..3]}", "%d %a" };
            yield return new object[]
                { $"{now.Day.ToString().PadLeft(2, ' ')} {now.AddDays(1).DayOfWeek.ToString()[..3]}", "%e %a" };
            yield return new object[]
                { $"{now.Day.ToString().PadLeft(2, ' ')} {now.AddDays(1).DayOfWeek.ToString()}", "%e %A" };
        }

        [Theory, MemberData(nameof(ShouldThrow_WhenIncoherentDayOfMonth_WithDayOfWeek_Data))]
        [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
        public void ShouldThrow_WhenIncoherentDayOfMonth_WithDayOfWeek(string input, string format)
        {
            // Arrange

            // Act
            Action act = () => Strftime.Parse(input, format);
            
            // Assert
            act.Should().Throw<FormatException>();
        }
        
        [Theory]
        [InlineData("11 12", "%d %e")]
        [InlineData("12 11", "%d %e")]
        public void ShouldThrow_WhenIncoherentDayOfMonth(string input, string format)
        {
            // Act
            Action act = () => _ = Strftime.Parse(input, format);
            
            // Assert
            act.Should().Throw<FormatException>();
        }
        
        [Theory]
        [InlineData("05", "%d", 5)]
        [InlineData(" 5", "%e", 5)]
        [InlineData("11 11", "%d %e", 11)]
        public void ReturnDayOfMonth_ForThisMonth_WhenNotPreciseInfoGiven(string input, string format, int day)
        {
            // Arrange
            var now = DateTime.Now;
            if (now.Day < day)
            {
                now = now.AddDays(day - now.Day);
            }
            else if(now.Day > day)
            {
                now = now.AddDays(-(now.Day - day));
            }
            
            // Act
            var res = Strftime.Parse(input, format);
            
            // Assert
            res.Day.Should().Be(now.Day);
        }
        
        [Theory]
        [InlineData("Monday Sat", "%A %a")]
        [InlineData("Saturday Mon", "%A %a")]
        [InlineData("Mon Friday", "%a %A")]
        public void ShouldThrow_WhenIncoherentDayOfWeek_Found(string input, string format)
        {
            // arrange

            // act
            Action act = () => _ = Strftime.Parse(input, format);
        
            // assert
            act.Should().ThrowExactly<FormatException>();
        }

        [Theory]
        [InlineData("Jan", "%b", 1)]
        [InlineData("Feb", "%b", 2)]
        [InlineData("Mar", "%b", 3)]
        [InlineData("Apr", "%b", 4)]
        [InlineData("May", "%b", 5)]
        [InlineData("Jun", "%b", 6)]
        [InlineData("Jul", "%b", 7)]
        [InlineData("Aug", "%b", 8)]
        [InlineData("Sep", "%b", 9)]
        [InlineData("Oct", "%b", 10)]
        [InlineData("Nov", "%b", 11)]
        [InlineData("Dec", "%b", 12)]
        public void Parse_Month_Abbreviated(string input, string format, int month)
        {
            var dt = Strftime.Parse(input, format);

            dt.Should().HaveMonth(month);
        }
        
        [Theory]
        [InlineData("January", "%B", 1)]
        [InlineData("February", "%B", 2)]
        [InlineData("March", "%B", 3)]
        [InlineData("April", "%B", 4)]
        [InlineData("May", "%B", 5)]
        [InlineData("June", "%B", 6)]
        [InlineData("July", "%B", 7)]
        [InlineData("August", "%B", 8)]
        [InlineData("September", "%B", 9)]
        [InlineData("October", "%B", 10)]
        [InlineData("November", "%B", 11)]
        [InlineData("December", "%B", 12)]
        public void Parse_Month_Full(string input, string format, int month)
        {
            var dt = Strftime.Parse(input, format);

            dt.Should().HaveMonth(month);
        }

        [Theory]
        [InlineData("08/23/01", "%D", 1, 8, 23)]
        [InlineData("08/23/23", "%D", 23, 8, 23)]
        public void ParseShortMmDdYy(string input, string format, int year, int month, int day)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveDay(day);
            res.Should().HaveMonth(month);
            res.Should()
                .HaveYear(int.Parse(DateTime.Now.ToString("yyyy")[..2] + 
                                    year.ToString().PadLeft(2, '0')));
        }
        
        [Theory]
        [InlineData("2001-08-23", "%F", 2001, 8, 23)]
        [InlineData("2101-07-21", "%F", 2101, 7, 21)]
        public void ParseShortYyyyMmDd(string input, string format, int year, int month, int day)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveDay(day);
            res.Should().HaveMonth(month);
            res.Should().HaveYear(year);
        }

        [Theory]
        [InlineData("20", "%C")]
        [InlineData("19", "%C")]
        [InlineData("21", "%C")]
        public void ParseYearDividedBy100(string input, string format)
        {
            var now = DateTime.Now;
            var intYear = now.Year / 100;
            var yearInput = int.Parse(input);
            if (yearInput < intYear)
            {
                while (intYear != yearInput)
                {
                    now = now.AddYears(-100);
                    intYear = now.Year / 100;
                }
            }
            else if (yearInput > intYear)
            {
                while (intYear != yearInput)
                {
                    now = now.AddYears(100);
                    intYear = now.Year / 100;
                }
            }


            var res = Strftime.Parse(input, format);

            res.Should().HaveYear(now.Year);
        }

        [Theory]
        [InlineData("00", "%H", 0)]
        [InlineData("01", "%H", 1)]
        [InlineData("02", "%H", 2)]
        [InlineData("03", "%H", 3)]
        [InlineData("04", "%H", 4)]
        [InlineData("05", "%H", 5)]
        [InlineData("06", "%H", 6)]
        [InlineData("07", "%H", 7)]
        [InlineData("08", "%H", 8)]
        [InlineData("09", "%H", 9)]
        [InlineData("10", "%H", 10)]
        [InlineData("11", "%H", 11)]
        [InlineData("12", "%H", 12)]
        [InlineData("13", "%H", 13)]
        [InlineData("14", "%H", 14)]
        [InlineData("15", "%H", 15)]
        [InlineData("16", "%H", 16)]
        [InlineData("17", "%H", 17)]
        [InlineData("18", "%H", 18)]
        [InlineData("19", "%H", 19)]
        [InlineData("20", "%H", 20)]
        [InlineData("21", "%H", 21)]
        [InlineData("22", "%H", 22)]
        [InlineData("23", "%H", 23)]
        public void ParseHour24(string input, string format, int hour)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveHour(hour);
        }

        [Theory]
        [InlineData("24", "%H")]
        [InlineData("33", "%H")]
        [InlineData("-1", "%H")]
        public void ShouldThrow_WhenInvalidHour23(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format);

            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("01 AM", "%I %p", 1)]
        [InlineData("02 AM", "%I %p", 2)]
        [InlineData("03 AM", "%I %p", 3)]
        [InlineData("04 AM", "%I %p", 4)]
        [InlineData("05 AM", "%I %p", 5)]
        [InlineData("06 AM", "%I %p", 6)]
        [InlineData("07 AM", "%I %p", 7)]
        [InlineData("08 AM", "%I %p", 8)]
        [InlineData("09 AM", "%I %p", 9)]
        [InlineData("10 AM", "%I %p", 10)]
        [InlineData("11 AM", "%I %p", 11)]
        [InlineData("12 AM", "%I %p", 0)]
        [InlineData("01 PM", "%I %p", 13)]
        [InlineData("02 PM", "%I %p", 14)]
        [InlineData("03 PM", "%I %p", 15)]
        [InlineData("04 PM", "%I %p", 16)]
        [InlineData("05 PM", "%I %p", 17)]
        [InlineData("06 PM", "%I %p", 18)]
        [InlineData("07 PM", "%I %p", 19)]
        [InlineData("08 PM", "%I %p", 20)]
        [InlineData("09 PM", "%I %p", 21)]
        [InlineData("10 PM", "%I %p", 22)]
        [InlineData("11 PM", "%I %p", 23)]
        [InlineData("12 PM", "%I %p", 12)]
        [InlineData("12\nPM", "%I%n%p", 12)]
        public void ParseHour12(string input, string format, int hour)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveHour(hour);
        }

        [Theory]
        [InlineData("366", "%j", 366)]
        [InlineData("150", "%j", 150)]
        [InlineData("001", "%j", 1)]
        public void ParseDayOfTheYear(string input, string format, int dayOfYear)
        {
            var now = DateTime.Now.AddDays(dayOfYear - DateTime.Now.DayOfYear);
            
            var res = Strftime.Parse(input, format);

            res.Should().HaveDay(now.Day);
            res.Should().HaveMonth(now.Month);
            res.Should().HaveYear(now.Year);
        }
        
        [Theory]
        [InlineData("01", "%m", 01)]
        [InlineData("02", "%m", 02)]
        [InlineData("03", "%m", 03)]
        [InlineData("04", "%m", 04)]
        [InlineData("05", "%m", 05)]
        [InlineData("06", "%m", 06)]
        [InlineData("07", "%m", 07)]
        [InlineData("08", "%m", 08)]
        [InlineData("09", "%m", 09)]
        [InlineData("10", "%m", 10)]
        [InlineData("11", "%m", 11)]
        [InlineData("12", "%m", 12)]
        public void ParseMonth(string input, string format, int expectedMonth)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveMonth(expectedMonth);
        }

        [Theory]
        [InlineData("13", "%m")]
        public void Should_NotParse_InvalidMonth(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format);

            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("00", "%M", 0)]
        [InlineData("59", "%M", 59)]
        [InlineData("35", "%M", 35)]
        public void ParseMinute(string input, string format, int expectedMinute)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveMinute(expectedMinute);
        }
        
        [Theory]
        [InlineData("00", "%S", 0)]
        [InlineData("59", "%S", 59)]
        [InlineData("60", "%S", 0)]
        public void ParseSecond(string input, string format, int expectedSecond)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveSecond(expectedSecond);
        }

        [Theory]
        [InlineData("14:55:02", "%T", 14, 55, 2)]
        [InlineData("14:55:60", "%T", 14, 56, 0)]
        [InlineData("23:01:01", "%T", 23, 1, 1)]
        [InlineData("00:01:01", "%T", 0, 1, 1)]
        public void ParseIsoTime(string input, string format, int expectedHour, int expectedMinute, int expectedSecond)
        {
            var res = Strftime.Parse(input, format);

            res.Should().HaveHour(expectedHour);
            res.Should().HaveMinute(expectedMinute);
            res.Should().HaveSecond(expectedSecond);
        }

        [Theory]
        [InlineData("1", "%u", DayOfWeek.Monday)]
        [InlineData("2", "%u", DayOfWeek.Tuesday)]
        [InlineData("3", "%u", DayOfWeek.Wednesday)]
        [InlineData("4", "%u", DayOfWeek.Thursday)]
        [InlineData("5", "%u", DayOfWeek.Friday)]
        [InlineData("6", "%u", DayOfWeek.Saturday)]
        [InlineData("7", "%u", DayOfWeek.Sunday)]
        public void ParseIsoDayOfWeek(string input, string format, DayOfWeek expectedDayOfWeek)
        {
            var res = Strftime.Parse(input, format);

            res.DayOfWeek.Should().Be(expectedDayOfWeek);
        }

        [Theory]
        [InlineData("0", "%w", DayOfWeek.Sunday)]
        [InlineData("1", "%w", DayOfWeek.Monday)]
        [InlineData("2", "%w", DayOfWeek.Tuesday)]
        [InlineData("3", "%w", DayOfWeek.Wednesday)]
        [InlineData("4", "%w", DayOfWeek.Thursday)]
        [InlineData("5", "%w", DayOfWeek.Friday)]
        [InlineData("6", "%w", DayOfWeek.Saturday)]
        public void ParseDayOfWeekSundayBased(string input, string format, DayOfWeek expectedDayOfWeek)
        {
            var res = Strftime.Parse(input, format);

            res.DayOfWeek.Should().Be(expectedDayOfWeek);
        }


        [Theory]
        [InlineData("00 AM", "%I %p")]
        [InlineData("00 PM", "%I %p")]
        [InlineData("13 PM", "%I %p")]
        [InlineData("13 AM", "%I %p")]
        [InlineData("12 GM", "%I %p")]
        public void ShouldThrow_When_Invalid12Hour(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format);

            act.Should().Throw<Exception>();
        }
        
        [Theory]
        [InlineData("Mon", "%a", DayOfWeek.Monday)]
        [InlineData("Monday", "%A", DayOfWeek.Monday)]
        [InlineData("Tue", "%a", DayOfWeek.Tuesday)]
        [InlineData("Wed", "%a", DayOfWeek.Wednesday)]
        [InlineData("Thu", "%a", DayOfWeek.Thursday)]
        [InlineData("Fri", "%a", DayOfWeek.Friday)]
        [InlineData("Sat", "%a", DayOfWeek.Saturday)]
        [InlineData("Sun", "%a", DayOfWeek.Sunday)]
        [InlineData("Monday Mon", "%A %a", DayOfWeek.Monday)]
        public void Return_DayOfWeek_ForThisWeek_WhenNot_PreciseInfo_AreGiven(string input, string format, DayOfWeek dayOfWeek)
        {
            // arrange
            var now = DateTime.Now;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            while (now.DayOfWeek != firstDayOfWeek) now = now.AddDays(-1);
            while (now.DayOfWeek != dayOfWeek) now = now.AddDays(1);
            
            // act
            var dt = Strftime.Parse(input, format);
        
            // assert
            dt.Should().HaveDay(now.Day);
            dt.Should().HaveMonth(now.Month);
            dt.Should().HaveYear(now.Year);
        }

        [Theory]
        [InlineData("Thu Aug 23 14:55:02 2001", "%c")]
        public void Parse_FullDate(string input, string format)
        {
            // Arrange
            
            // Act
            var dt = Strftime.Parse(input, format);
            
            // Assert
            dt.Should().HaveDay(23);
            dt.Should().HaveMonth(8);
            dt.Should().NotHaveYear(2001);
            dt.Should().HaveHour(14);
            dt.Should().HaveMinute(55);
            dt.Should().HaveSecond(02);
        }
    }
}