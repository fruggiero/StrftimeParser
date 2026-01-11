using System;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    /// <summary>
    /// Tests for POSIX strptime compatibility.
    /// POSIX specifies that text-based fields (month names, day names) should be case-insensitive.
    /// </summary>
    public class PosixCompatibilityTest
    {
        private readonly CultureInfo _enUs = CultureInfo.GetCultureInfo("en-US");
        private readonly CultureInfo _itIt = CultureInfo.GetCultureInfo("it-IT");
        private readonly CultureInfo _frFr = CultureInfo.GetCultureInfo("fr-FR");

        #region Case-Insensitive Month Names (%B) - en-US

        [Theory]
        [InlineData("JANUARY", 1)]
        [InlineData("january", 1)]
        [InlineData("JaNuArY", 1)]
        [InlineData("FEBRUARY", 2)]
        [InlineData("february", 2)]
        [InlineData("MARCH", 3)]
        [InlineData("march", 3)]
        [InlineData("APRIL", 4)]
        [InlineData("april", 4)]
        [InlineData("MAY", 5)]
        [InlineData("may", 5)]
        [InlineData("JUNE", 6)]
        [InlineData("june", 6)]
        [InlineData("JULY", 7)]
        [InlineData("july", 7)]
        [InlineData("AUGUST", 8)]
        [InlineData("august", 8)]
        [InlineData("SEPTEMBER", 9)]
        [InlineData("september", 9)]
        [InlineData("OCTOBER", 10)]
        [InlineData("october", 10)]
        [InlineData("NOVEMBER", 11)]
        [InlineData("november", 11)]
        [InlineData("DECEMBER", 12)]
        [InlineData("december", 12)]
        public void Parse_FullMonth_CaseInsensitive_EnUs(string input, int expectedMonth)
        {
            var result = Strftime.Parse(input, "%B", _enUs);
            result.Month.Should().Be(expectedMonth);
        }

        #endregion

        #region Case-Insensitive Abbreviated Month Names (%b) - en-US

        [Theory]
        [InlineData("JAN", 1)]
        [InlineData("jan", 1)]
        [InlineData("Jan", 1)]
        [InlineData("FEB", 2)]
        [InlineData("feb", 2)]
        [InlineData("MAR", 3)]
        [InlineData("mar", 3)]
        [InlineData("APR", 4)]
        [InlineData("apr", 4)]
        [InlineData("MAY", 5)]
        [InlineData("may", 5)]
        [InlineData("JUN", 6)]
        [InlineData("jun", 6)]
        [InlineData("JUL", 7)]
        [InlineData("jul", 7)]
        [InlineData("AUG", 8)]
        [InlineData("aug", 8)]
        [InlineData("SEP", 9)]
        [InlineData("sep", 9)]
        [InlineData("OCT", 10)]
        [InlineData("oct", 10)]
        [InlineData("NOV", 11)]
        [InlineData("nov", 11)]
        [InlineData("DEC", 12)]
        [InlineData("dec", 12)]
        public void Parse_AbbreviatedMonth_CaseInsensitive_EnUs(string input, int expectedMonth)
        {
            var result = Strftime.Parse(input, "%b", _enUs);
            result.Month.Should().Be(expectedMonth);
        }

        #endregion

        #region Case-Insensitive Day Names (%A) - en-US

        [Theory]
        [InlineData("MONDAY", DayOfWeek.Monday)]
        [InlineData("monday", DayOfWeek.Monday)]
        [InlineData("MoNdAy", DayOfWeek.Monday)]
        [InlineData("TUESDAY", DayOfWeek.Tuesday)]
        [InlineData("tuesday", DayOfWeek.Tuesday)]
        [InlineData("WEDNESDAY", DayOfWeek.Wednesday)]
        [InlineData("wednesday", DayOfWeek.Wednesday)]
        [InlineData("THURSDAY", DayOfWeek.Thursday)]
        [InlineData("thursday", DayOfWeek.Thursday)]
        [InlineData("FRIDAY", DayOfWeek.Friday)]
        [InlineData("friday", DayOfWeek.Friday)]
        [InlineData("SATURDAY", DayOfWeek.Saturday)]
        [InlineData("saturday", DayOfWeek.Saturday)]
        [InlineData("SUNDAY", DayOfWeek.Sunday)]
        [InlineData("sunday", DayOfWeek.Sunday)]
        public void Parse_FullDayOfWeek_CaseInsensitive_EnUs(string input, DayOfWeek expectedDay)
        {
            var result = Strftime.Parse(input, "%A", _enUs);
            result.DayOfWeek.Should().Be(expectedDay);
        }

        #endregion

        #region Case-Insensitive Abbreviated Day Names (%a) - en-US

        [Theory]
        [InlineData("MON", DayOfWeek.Monday)]
        [InlineData("mon", DayOfWeek.Monday)]
        [InlineData("Mon", DayOfWeek.Monday)]
        [InlineData("TUE", DayOfWeek.Tuesday)]
        [InlineData("tue", DayOfWeek.Tuesday)]
        [InlineData("WED", DayOfWeek.Wednesday)]
        [InlineData("wed", DayOfWeek.Wednesday)]
        [InlineData("THU", DayOfWeek.Thursday)]
        [InlineData("thu", DayOfWeek.Thursday)]
        [InlineData("FRI", DayOfWeek.Friday)]
        [InlineData("fri", DayOfWeek.Friday)]
        [InlineData("SAT", DayOfWeek.Saturday)]
        [InlineData("sat", DayOfWeek.Saturday)]
        [InlineData("SUN", DayOfWeek.Sunday)]
        [InlineData("sun", DayOfWeek.Sunday)]
        public void Parse_AbbreviatedDayOfWeek_CaseInsensitive_EnUs(string input, DayOfWeek expectedDay)
        {
            var result = Strftime.Parse(input, "%a", _enUs);
            result.DayOfWeek.Should().Be(expectedDay);
        }

        #endregion

        #region Case-Insensitive AM/PM (%p)

        [Theory]
        [InlineData("01 AM", 1)]
        [InlineData("01 am", 1)]
        [InlineData("01 Am", 1)]
        [InlineData("01 aM", 1)]
        [InlineData("01 PM", 13)]
        [InlineData("01 pm", 13)]
        [InlineData("01 Pm", 13)]
        [InlineData("01 pM", 13)]
        public void Parse_AmPm_CaseInsensitive(string input, int expectedHour)
        {
            var result = Strftime.Parse(input, "%I %p", _enUs);
            result.Hour.Should().Be(expectedHour);
        }

        #endregion

        #region Case-Insensitive Non-English Cultures (GenericFormatter)

        [Theory]
        [InlineData("GENNAIO", 1)]
        [InlineData("gennaio", 1)]
        [InlineData("Gennaio", 1)]
        [InlineData("FEBBRAIO", 2)]
        [InlineData("febbraio", 2)]
        [InlineData("DICEMBRE", 12)]
        [InlineData("dicembre", 12)]
        public void Parse_FullMonth_CaseInsensitive_ItIt(string input, int expectedMonth)
        {
            var result = Strftime.Parse(input, "%B", _itIt);
            result.Month.Should().Be(expectedMonth);
        }

        [Theory]
        [InlineData("GEN", 1)]
        [InlineData("gen", 1)]
        [InlineData("Gen", 1)]
        [InlineData("DIC", 12)]
        [InlineData("dic", 12)]
        public void Parse_AbbreviatedMonth_CaseInsensitive_ItIt(string input, int expectedMonth)
        {
            var result = Strftime.Parse(input, "%b", _itIt);
            result.Month.Should().Be(expectedMonth);
        }

        [Theory]
        [InlineData("LUNEDÌ", DayOfWeek.Monday)]
        [InlineData("lunedì", DayOfWeek.Monday)]
        [InlineData("Lunedì", DayOfWeek.Monday)]
        [InlineData("DOMENICA", DayOfWeek.Sunday)]
        [InlineData("domenica", DayOfWeek.Sunday)]
        public void Parse_FullDayOfWeek_CaseInsensitive_ItIt(string input, DayOfWeek expectedDay)
        {
            var result = Strftime.Parse(input, "%A", _itIt);
            result.DayOfWeek.Should().Be(expectedDay);
        }

        [Theory]
        [InlineData("LUN", DayOfWeek.Monday)]
        [InlineData("lun", DayOfWeek.Monday)]
        [InlineData("Lun", DayOfWeek.Monday)]
        [InlineData("DOM", DayOfWeek.Sunday)]
        [InlineData("dom", DayOfWeek.Sunday)]
        public void Parse_AbbreviatedDayOfWeek_CaseInsensitive_ItIt(string input, DayOfWeek expectedDay)
        {
            var result = Strftime.Parse(input, "%a", _itIt);
            result.DayOfWeek.Should().Be(expectedDay);
        }

        #endregion

        #region French Culture Case-Insensitivity

        [Theory]
        [InlineData("JANVIER", 1)]
        [InlineData("janvier", 1)]
        [InlineData("Janvier", 1)]
        [InlineData("DÉCEMBRE", 12)]
        [InlineData("décembre", 12)]
        public void Parse_FullMonth_CaseInsensitive_FrFr(string input, int expectedMonth)
        {
            var result = Strftime.Parse(input, "%B", _frFr);
            result.Month.Should().Be(expectedMonth);
        }

        #endregion

        #region Percent Literal (%%)

        [Theory]
        [InlineData("100%", "100%%")]
        [InlineData("50% off", "50%% off")]
        [InlineData("%2023", "%%%Y")]
        public void Parse_PercentLiteral(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format, _enUs);
            act.Should().NotThrow();
        }

        [Fact]
        public void Parse_PercentLiteral_WithYear()
        {
            var result = Strftime.Parse("%2023", "%%%Y", _enUs);
            result.Year.Should().Be(2023);
        }

        #endregion

        #region Whitespace Handling (%t, %n)

        [Theory]
        [InlineData("2023\t10", "%Y%t%m")]
        [InlineData("2023\t\t10", "%Y%t%t%m")]
        public void Parse_Tab_Whitespace(string input, string format)
        {
            var result = Strftime.Parse(input, format, _enUs);
            result.Year.Should().Be(2023);
            result.Month.Should().Be(10);
        }

        [Theory]
        [InlineData("2023\n10", "%Y%n%m")]
        [InlineData("2023\n\n10", "%Y%n%n%m")]
        public void Parse_Newline_Whitespace(string input, string format)
        {
            var result = Strftime.Parse(input, format, _enUs);
            result.Year.Should().Be(2023);
            result.Month.Should().Be(10);
        }

        #endregion

        #region Numeric Field Edge Cases

        [Theory]
        [InlineData("00", "%d")]  // Invalid day
        [InlineData("32", "%d")]  // Invalid day
        public void Parse_InvalidDay_ShouldThrow(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format, _enUs);
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("00", "%m")]  // Invalid month
        [InlineData("13", "%m")]  // Invalid month
        public void Parse_InvalidMonth_ShouldThrow(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format, _enUs);
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("60", "%M")]  // Invalid minute
        public void Parse_InvalidMinute_ShouldThrow(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format, _enUs);
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("61", "%S")]  // Invalid second (60 is valid for leap seconds per POSIX)
        public void Parse_InvalidSecond_ShouldThrow(string input, string format)
        {
            Action act = () => Strftime.Parse(input, format, _enUs);
            act.Should().Throw<Exception>();
        }

        #endregion

        #region Combined Format Case-Insensitivity

        [Fact]
        public void Parse_FullDateFormat_CaseInsensitive()
        {
            // %c format with all uppercase day and month names
            var result = Strftime.Parse("THU AUG 23 14:55:02 2001", "%c", _enUs);
            
            result.DayOfWeek.Should().Be(DayOfWeek.Thursday);
            result.Month.Should().Be(8);
            result.Day.Should().Be(23);
            result.Hour.Should().Be(14);
            result.Minute.Should().Be(55);
            result.Second.Should().Be(2);
            result.Year.Should().Be(2001);
        }

        [Fact]
        public void Parse_FullDateFormat_LowerCase()
        {
            var result = Strftime.Parse("thu aug 23 14:55:02 2001", "%c", _enUs);
            
            result.DayOfWeek.Should().Be(DayOfWeek.Thursday);
            result.Month.Should().Be(8);
        }

        [Theory]
        [InlineData("JANUARY 15, 2024", "%B %d, %Y", 1, 15, 2024)]
        [InlineData("january 15, 2024", "%B %d, %Y", 1, 15, 2024)]
        [InlineData("JaNuArY 15, 2024", "%B %d, %Y", 1, 15, 2024)]
        public void Parse_DateWithFullMonth_CaseInsensitive(string input, string format, int month, int day, int year)
        {
            var result = Strftime.Parse(input, format, _enUs);
            
            result.Month.Should().Be(month);
            result.Day.Should().Be(day);
            result.Year.Should().Be(year);
        }

        #endregion

        #region Space-Padded Day (%e) Edge Cases

        [Theory]
        [InlineData(" 1", "%e", 1)]
        [InlineData(" 9", "%e", 9)]
        [InlineData("10", "%e", 10)]
        [InlineData("31", "%e", 31)]
        public void Parse_SpacePaddedDay(string input, string format, int expectedDay)
        {
            var result = Strftime.Parse(input, format, _enUs);
            result.Day.Should().Be(expectedDay);
        }

        #endregion
    }
}
