using System;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    public class EnUsFormatterTest
    {
        private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("en-US");

        #region Full Month Tests (%B)
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
        public void Parse_FullMonth_ShouldReturnCorrectMonth(string input, string format, int expectedMonth)
        {
            // Act
            var result = Strftime.Parse(input, format, _culture);

            // Assert
            result.Month.Should().Be(expectedMonth);
        }
        #endregion

        #region Abbreviated Month Tests (%b)
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
        public void Parse_AbbreviatedMonth_ShouldReturnCorrectMonth(string input, string format, int expectedMonth)
        {
            // Act
            var result = Strftime.Parse(input, format, _culture);

            // Assert
            result.Month.Should().Be(expectedMonth);
        }
        #endregion

        #region Full Day of Week Tests (%A)
        [Theory]
        [InlineData("Monday", "%A", DayOfWeek.Monday)]
        [InlineData("Tuesday", "%A", DayOfWeek.Tuesday)]
        [InlineData("Wednesday", "%A", DayOfWeek.Wednesday)]
        [InlineData("Thursday", "%A", DayOfWeek.Thursday)]
        [InlineData("Friday", "%A", DayOfWeek.Friday)]
        [InlineData("Saturday", "%A", DayOfWeek.Saturday)]
        [InlineData("Sunday", "%A", DayOfWeek.Sunday)]
        public void Parse_FullDayOfWeek_ShouldReturnCorrectDayOfWeek(string input, string format, DayOfWeek expectedDayOfWeek)
        {
            // Act
            var result = Strftime.Parse(input, format, _culture);

            // Assert
            result.DayOfWeek.Should().Be(expectedDayOfWeek);
        }
        #endregion

        #region Abbreviated Day of Week Tests (%a)
        [Theory]
        [InlineData("Mon", "%a", DayOfWeek.Monday)]
        [InlineData("Tue", "%a", DayOfWeek.Tuesday)]
        [InlineData("Wed", "%a", DayOfWeek.Wednesday)]
        [InlineData("Thu", "%a", DayOfWeek.Thursday)]
        [InlineData("Fri", "%a", DayOfWeek.Friday)]
        [InlineData("Sat", "%a", DayOfWeek.Saturday)]
        [InlineData("Sun", "%a", DayOfWeek.Sunday)]
        public void Parse_AbbreviatedDayOfWeek_ShouldReturnCorrectDayOfWeek(string input, string format, DayOfWeek expectedDayOfWeek)
        {
            // Act
            var result = Strftime.Parse(input, format, _culture);

            // Assert
            result.DayOfWeek.Should().Be(expectedDayOfWeek);
        }
        #endregion

        #region Bug-Specific Tests
        [Fact]
        public void BugRepro_February_IndexIncrement_ShouldParseCompleteDate()
        {
            // This test specifically targets the bug in EnUsFormatter.ConsumeFullMonth (line 108)
            // where February parsing increments inputIndex by "January".Length (7) 
            // instead of "February".Length (8), causing subsequent parsing to fail.
            
            // Arrange
            string input = "February 15, 2024";
            string format = "%B %d, %Y";

            // Act
            var result = Strftime.Parse(input, format, _culture);

            // Assert
            result.Month.Should().Be(2, "February should be parsed as month 2");
            result.Day.Should().Be(15, "Day should be parsed correctly after February (bug causes this to fail)");
            result.Year.Should().Be(2024, "Year should be parsed correctly");
        }

        [Theory]
        [InlineData("February 01, 2024", "%B %d, %Y")]
        [InlineData("February 29, 2024", "%B %d, %Y")] // Leap year test
        [InlineData("February 2024", "%B %Y")]
        public void Parse_February_WithVariousFormats_ShouldHandleCorrectly(string input, string format)
        {
            // Verifies that February parsing works in various real-world scenarios
            // where the index increment bug could cause failures
            
            // Act & Assert
            Action act = () => Strftime.Parse(input, format, _culture);
            act.Should().NotThrow("February should be parsed correctly in various formats");
        }
        #endregion

        #region Edge Cases
        [Theory]
        [InlineData("May", "%B")] // Shortest month name
        [InlineData("September", "%B")] // Longest month name
        [InlineData("May 01", "%B %d")] // Shortest month with day
        [InlineData("September 30", "%B %d")] // Longest month with day
        public void Parse_MonthEdgeCases_ShouldHandleCorrectly(string input, string format)
        {
            // Act & Assert
            Action act = () => Strftime.Parse(input, format, _culture);
            act.Should().NotThrow("Month edge cases should be handled correctly");
        }
        #endregion
    }
}
