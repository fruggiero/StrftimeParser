using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using StrftimeParser;
using Xunit;

namespace StrftimeParserTest
{
    public class ToStringTest
    {
        private readonly CultureInfo _culture = new("en-US");

        public static IEnumerable<object[]> FormatTestData =>
            new List<object[]>
            {
                new object[] { "%Y", "1970" },
                new object[] { "%y", "70" },
                new object[] { "%S", "05" },
                new object[] { "%t", "\t" },
                new object[] { "%T", "03:04:05" },
                new object[] { "%u", "5" },
                new object[] { "%n", "\n" },
                new object[] { "%m", "01" },
                new object[] { "%M", "04" },
                new object[] { "%I", "03" },
                new object[] { "%p", "AM" },
                new object[] { "%H", "03" },
                new object[] { "%B", "January" },
                new object[] { "%b", "Jan" },
                new object[] { "%C", "19" },
                new object[] { "%c", "Fri Jan 02 03:04:05 1970" },
                new object[] { "%a", "Fri" },
                new object[] { "%A", "Friday" },
                new object[] { "%d", "02" },
                new object[] { "%e", " 2" },
                new object[] { "%D", "01/02/70" },
                new object[] { "%F", "1970-01-02" },
                new object[] { "%j", "002" },
                new object[] { "%w", "5" }
            };

        [Theory]
        [MemberData(nameof(FormatTestData))]
        public void Should_ConvertToString(string format, string expectedResult)
        {
            var dt = new DateTime(1970, 1, 2, 3, 4, 5);

            var res = Strftime.ToString(dt, format, _culture);

            res.Should().Be(expectedResult);
        }
        
        [Theory]
        [MemberData(nameof(FormatTestData))]
        public void Should_ConvertToString_UsingExtensionMethod(string format, string expectedResult)
        {
            var dt = new DateTime(1970, 1, 2, 3, 4, 5);

            var res = dt.ToStrftimeString(format, _culture);

            res.Should().Be(expectedResult);
        }

        
        [Theory]
        [InlineData("asd %Y asd", "asd 1970 asd")]
        [InlineData("asd %Y", "asd 1970")]
        [InlineData("%Y asd", "1970 asd")]
        public void Should_LeaveExtraChars(string format, string expectedResult)
        {
            var dt = new DateTime(1970, 1, 2, 3, 4, 5);

            var res = Strftime.ToString(dt, format, _culture);

            res.Should().Be(expectedResult);
        }
    }
}