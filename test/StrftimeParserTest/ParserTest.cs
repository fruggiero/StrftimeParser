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
    }
}