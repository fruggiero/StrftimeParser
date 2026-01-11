using System;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    /// <summary>
    /// Tests for TryParse methods.
    /// </summary>
    public class TryParseTest
    {
        private readonly CultureInfo _enUs = CultureInfo.GetCultureInfo("en-US");

        #region TryParse Success Cases

        [Theory]
        [InlineData("2023-10-15", "%Y-%m-%d", 2023, 10, 15)]
        [InlineData("2024-01-01", "%Y-%m-%d", 2024, 1, 1)]
        [InlineData("1999-12-31", "%Y-%m-%d", 1999, 12, 31)]
        public void TryParse_ValidInput_ReturnsTrue(string input, string format, int year, int month, int day)
        {
            // Act
            var success = Strftime.TryParse(input, format, _enUs, out var result);

            // Assert
            success.Should().BeTrue();
            result.Year.Should().Be(year);
            result.Month.Should().Be(month);
            result.Day.Should().Be(day);
        }

        [Theory]
        [InlineData("14:30:45", "%H:%M:%S", 14, 30, 45)]
        [InlineData("00:00:00", "%H:%M:%S", 0, 0, 0)]
        [InlineData("23:59:59", "%H:%M:%S", 23, 59, 59)]
        public void TryParse_ValidTime_ReturnsTrue(string input, string format, int hour, int minute, int second)
        {
            // Act
            var success = Strftime.TryParse(input, format, _enUs, out var result);

            // Assert
            success.Should().BeTrue();
            result.Hour.Should().Be(hour);
            result.Minute.Should().Be(minute);
            result.Second.Should().Be(second);
        }

        [Theory]
        [InlineData("January", "%B", 1)]
        [InlineData("FEBRUARY", "%B", 2)]
        [InlineData("december", "%B", 12)]
        public void TryParse_ValidMonth_ReturnsTrue(string input, string format, int month)
        {
            // Act
            var success = Strftime.TryParse(input, format, _enUs, out var result);

            // Assert
            success.Should().BeTrue();
            result.Month.Should().Be(month);
        }

        #endregion

        #region TryParse Failure Cases

        [Theory]
        [InlineData("invalid", "%Y-%m-%d")]
        [InlineData("2023-13-01", "%Y-%m-%d")]  // Invalid month
        [InlineData("2023-01-32", "%Y-%m-%d")]  // Invalid day
        [InlineData("25:00:00", "%H:%M:%S")]    // Invalid hour
        [InlineData("NotAMonth", "%B")]         // Invalid month name
        public void TryParse_InvalidInput_ReturnsFalse(string input, string format)
        {
            // Act
            var success = Strftime.TryParse(input, format, _enUs, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TryParse_NullInput_ReturnsFalse()
        {
            // Act
            var success = Strftime.TryParse(null, "%Y-%m-%d", _enUs, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TryParse_NullFormat_ReturnsFalse()
        {
            // Act
            var success = Strftime.TryParse("2023-10-15", null, _enUs, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TryParse_NullCulture_ReturnsFalse()
        {
            // Act
            var success = Strftime.TryParse("2023-10-15", "%Y-%m-%d", null, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        #endregion

        #region TryParse Without Culture (Uses CurrentCulture)

        [Fact]
        public void TryParse_WithoutCulture_UsesCurrentCulture()
        {
            // Act
            var success = Strftime.TryParse("2023-10-15", "%Y-%m-%d", out var result);

            // Assert
            success.Should().BeTrue();
            result.Year.Should().Be(2023);
            result.Month.Should().Be(10);
            result.Day.Should().Be(15);
        }

        [Fact]
        public void TryParse_WithoutCulture_InvalidInput_ReturnsFalse()
        {
            // Act
            var success = Strftime.TryParse("invalid", "%Y-%m-%d", out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        #endregion

        #region TryParse vs Parse Equivalence

        [Theory]
        [InlineData("2023-10-15 14:30:45", "%Y-%m-%d %H:%M:%S")]
        [InlineData("Thu Aug 23 14:55:02 2001", "%c")]
        [InlineData("08/23/01", "%D")]
        public void TryParse_WhenSuccessful_MatchesParse(string input, string format)
        {
            // Arrange
            var parseResult = Strftime.Parse(input, format, _enUs);

            // Act
            var success = Strftime.TryParse(input, format, _enUs, out var tryParseResult);

            // Assert
            success.Should().BeTrue();
            tryParseResult.Should().Be(parseResult);
        }

        #endregion

        #region TryParse with CoherenceCheck

        [Theory]
        [InlineData("2023 24", "%Y %y")]  // Incoherent years
        [InlineData("Monday Sat", "%A %a")]  // Incoherent days
        public void TryParse_WithCoherenceCheck_IncoherentInput_ReturnsFalse(string input, string format)
        {
            // Act
            var success = Strftime.TryParse(input, format, _enUs, out var result, coherenceCheck: true);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TryParse_WithCoherenceCheck_CoherentInput_ReturnsTrue()
        {
            // Act
            var success = Strftime.TryParse("2023 23", "%Y %y", _enUs, out var result, coherenceCheck: true);

            // Assert
            success.Should().BeTrue();
            result.Year.Should().Be(2023);
        }

        #endregion

        #region ReadOnlySpan Overloads

        [Theory]
        [InlineData("2023-10-15", "%Y-%m-%d", 2023, 10, 15)]
        [InlineData("2024-01-01", "%Y-%m-%d", 2024, 1, 1)]
        [InlineData("1999-12-31", "%Y-%m-%d", 1999, 12, 31)]
        public void TryParse_Span_ValidInput_ReturnsTrue(string input, string format, int year, int month, int day)
        {
            // Act
            var success = Strftime.TryParse(input.AsSpan(), format.AsSpan(), _enUs, out var result);

            // Assert
            success.Should().BeTrue();
            result.Year.Should().Be(year);
            result.Month.Should().Be(month);
            result.Day.Should().Be(day);
        }

        [Theory]
        [InlineData("invalid", "%Y-%m-%d")]
        [InlineData("2023-13-01", "%Y-%m-%d")]
        [InlineData("NotAMonth", "%B")]
        public void TryParse_Span_InvalidInput_ReturnsFalse(string input, string format)
        {
            // Act
            var success = Strftime.TryParse(input.AsSpan(), format.AsSpan(), _enUs, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TryParse_Span_WithoutCulture_UsesCurrentCulture()
        {
            // Act
            var success = Strftime.TryParse("2023-10-15".AsSpan(), "%Y-%m-%d".AsSpan(), out var result);

            // Assert
            success.Should().BeTrue();
            result.Year.Should().Be(2023);
            result.Month.Should().Be(10);
            result.Day.Should().Be(15);
        }

        [Fact]
        public void TryParse_Span_NullCulture_ReturnsFalse()
        {
            // Act
            var success = Strftime.TryParse("2023-10-15".AsSpan(), "%Y-%m-%d".AsSpan(), null, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Theory]
        [InlineData("2023-10-15 14:30:45", "%Y-%m-%d %H:%M:%S")]
        [InlineData("Thu Aug 23 14:55:02 2001", "%c")]
        public void Parse_Span_ValidInput_ReturnsDateTime(string input, string format)
        {
            // Arrange
            var expected = Strftime.Parse(input, format, _enUs);

            // Act
            var result = Strftime.Parse(input.AsSpan(), format.AsSpan(), _enUs);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Parse_Span_WithoutCulture_UsesCurrentCulture()
        {
            // Arrange
            var expected = Strftime.Parse("2023-10-15", "%Y-%m-%d");

            // Act
            var result = Strftime.Parse("2023-10-15".AsSpan(), "%Y-%m-%d".AsSpan());

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Parse_Span_NullCulture_ThrowsArgumentNullException()
        {
            // Act
            Action act = () => Strftime.Parse("2023-10-15".AsSpan(), "%Y-%m-%d".AsSpan(), null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("culture");
        }

        [Theory]
        [InlineData("invalid", "%Y-%m-%d")]
        [InlineData("NotAMonth", "%B")]
        public void Parse_Span_InvalidInput_ThrowsException(string input, string format)
        {
            // Act
            Action act = () => Strftime.Parse(input.AsSpan(), format.AsSpan(), _enUs);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("2023 24", "%Y %y")]
        public void Parse_Span_WithCoherenceCheck_IncoherentInput_ThrowsFormatException(string input, string format)
        {
            // Act
            Action act = () => Strftime.Parse(input.AsSpan(), format.AsSpan(), _enUs, coherenceCheck: true);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void TryParse_Span_WithCoherenceCheck_IncoherentInput_ReturnsFalse()
        {
            // Act
            var success = Strftime.TryParse("2023 24".AsSpan(), "%Y %y".AsSpan(), _enUs, out var result, coherenceCheck: true);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void TryParse_Span_EquivalentToString_WhenSuccessful()
        {
            // Arrange
            const string input = "2023-10-15 14:30:45";
            const string format = "%Y-%m-%d %H:%M:%S";
            Strftime.TryParse(input, format, _enUs, out var stringResult);

            // Act
            var success = Strftime.TryParse(input.AsSpan(), format.AsSpan(), _enUs, out var spanResult);

            // Assert
            success.Should().BeTrue();
            spanResult.Should().Be(stringResult);
        }

        #endregion
    }
}
