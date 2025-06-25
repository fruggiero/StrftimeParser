using System;
using System.Globalization;

namespace StrftimeParser.Formatters
{
    internal class EnUsFormatter : Formatter
    {
        protected override CultureInfo Culture => CultureInfo.GetCultureInfo("en-US");

        public override ReadOnlySpan<char> ConsumeAbbreviatedDayOfWeek(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var abbrWeekDay = input.Slice(inputIndex, 3);
            inputIndex += 3;
            return abbrWeekDay;
        }

        public override ReadOnlySpan<char> ConsumeAbbreviatedMonth(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            var month = input.Slice(inputIndex, 3);
            inputIndex += 3;
            return month;
        }

        public override int ParseMonthFull(ReadOnlySpan<char> input)
        {
            if (input.Equals("january".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 1;
            if (input.Equals("february".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 2;
            if (input.Equals("march".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 3;
            if (input.Equals("april".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 4;
            if (input.Equals("may".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 5;
            if (input.Equals("june".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 6;
            if (input.Equals("july".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 7;
            if (input.Equals("august".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 8;
            if (input.Equals("september".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 9;
            if (input.Equals("october".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 10;
            if (input.Equals("november".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 11;
            if (input.Equals("december".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return 12;

            throw new FormatException("Unrecognized month name for this locale");
        }


        public override int ParseMonthAbbreviated(ReadOnlySpan<char> input)
        {
            if (input.Length != 3)
                throw new FormatException("Invalid abbreviated month length");

            // Use bitwise OR with 0x20 for fast case-insensitive comparison
            char c1 = (char)(input[0] | 0x20);
            char c2 = (char)(input[1] | 0x20);
            char c3 = (char)(input[2] | 0x20);

            switch (c1)
            {
                case 'j':
                    if (c2 == 'a' && c3 == 'n') return 1; // jan
                    if (c2 == 'u' && c3 == 'n') return 6; // jun
                    if (c2 == 'u' && c3 == 'l') return 7; // jul
                    break;
                case 'f':
                    if (c2 == 'e' && c3 == 'b') return 2; // feb
                    break;
                case 'm':
                    if (c2 == 'a' && c3 == 'r') return 3; // mar
                    if (c2 == 'a' && c3 == 'y') return 5; // may
                    break;
                case 'a':
                    if (c2 == 'p' && c3 == 'r') return 4; // apr
                    if (c2 == 'u' && c3 == 'g') return 8; // aug
                    break;
                case 's':
                    if (c2 == 'e' && c3 == 'p') return 9; // sep
                    break;
                case 'o':
                    if (c2 == 'c' && c3 == 't') return 10; // oct
                    break;
                case 'n':
                    if (c2 == 'o' && c3 == 'v') return 11; // nov
                    break;
                case 'd':
                    if (c2 == 'e' && c3 == 'c') return 12; // dec
                    break;
            }

            throw new FormatException("Unrecognized month abbreviated for this locale");
        }


        public override ReadOnlySpan<char> ConsumeFullMonth(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            if (inputIndex >= input.Length)
                throw new FormatException("Unexpected end of input");

            char firstChar = (char)(input[inputIndex] | 0x20); // Fast lowercase conversion

            switch (firstChar)
            {
                case 'j':
                    if (inputIndex + 1 < input.Length)
                    {
                        char secondChar = (char)(input[inputIndex + 1] | 0x20);
                        switch (secondChar)
                        {
                            case 'a':
                                // January
                                if (CheckStringMatch(input, inputIndex, "january"))
                                {
                                    var result = input.Slice(inputIndex, 7);
                                    inputIndex += 7;
                                    return result;
                                }

                                break;
                            case 'u':
                                if (inputIndex + 2 < input.Length)
                                {
                                    char thirdChar = (char)(input[inputIndex + 2] | 0x20);
                                    switch (thirdChar)
                                    {
                                        case 'l':
                                            // July
                                            if (CheckStringMatch(input, inputIndex, "july"))
                                            {
                                                var result = input.Slice(inputIndex, 4);
                                                inputIndex += 4;
                                                return result;
                                            }

                                            break;
                                        case 'n':
                                            // June
                                            if (CheckStringMatch(input, inputIndex, "june"))
                                            {
                                                var result = input.Slice(inputIndex, 4);
                                                inputIndex += 4;
                                                return result;
                                            }

                                            break;
                                    }
                                }

                                break;
                        }
                    }

                    break;

                case 'f':
                    // February
                    if (CheckStringMatch(input, inputIndex, "february"))
                    {
                        var result = input.Slice(inputIndex, 8);
                        inputIndex += 8;
                        return result;
                    }

                    break;

                case 'm':
                    if (inputIndex + 2 < input.Length)
                    {
                        char thirdChar = (char)(input[inputIndex + 2] | 0x20);
                        switch (thirdChar)
                        {
                            case 'r':
                                // March
                                if (CheckStringMatch(input, inputIndex, "march"))
                                {
                                    var result = input.Slice(inputIndex, 5);
                                    inputIndex += 5;
                                    return result;
                                }

                                break;
                            case 'y':
                                // May
                                if (CheckStringMatch(input, inputIndex, "may"))
                                {
                                    var result = input.Slice(inputIndex, 3);
                                    inputIndex += 3;
                                    return result;
                                }

                                break;
                        }
                    }

                    break;

                case 'a':
                    if (inputIndex + 1 < input.Length)
                    {
                        char secondChar = (char)(input[inputIndex + 1] | 0x20);
                        switch (secondChar)
                        {
                            case 'p':
                                // April
                                if (CheckStringMatch(input, inputIndex, "april"))
                                {
                                    var result = input.Slice(inputIndex, 5);
                                    inputIndex += 5;
                                    return result;
                                }

                                break;
                            case 'u':
                                // August
                                if (CheckStringMatch(input, inputIndex, "august"))
                                {
                                    var result = input.Slice(inputIndex, 6);
                                    inputIndex += 6;
                                    return result;
                                }

                                break;
                        }
                    }

                    break;

                case 's':
                    // September
                    if (CheckStringMatch(input, inputIndex, "september"))
                    {
                        var result = input.Slice(inputIndex, 9);
                        inputIndex += 9;
                        return result;
                    }

                    break;

                case 'o':
                    // October
                    if (CheckStringMatch(input, inputIndex, "october"))
                    {
                        var result = input.Slice(inputIndex, 7);
                        inputIndex += 7;
                        return result;
                    }

                    break;

                case 'n':
                    // November
                    if (CheckStringMatch(input, inputIndex, "november"))
                    {
                        var result = input.Slice(inputIndex, 8);
                        inputIndex += 8;
                        return result;
                    }

                    break;

                case 'd':
                    // December
                    if (CheckStringMatch(input, inputIndex, "december"))
                    {
                        var result = input.Slice(inputIndex, 8);
                        inputIndex += 8;
                        return result;
                    }

                    break;
            }

            throw new FormatException("Unrecognized full month format for this locale");
        }


        public override DayOfWeek ParseDayOfWeekAbbreviated(ReadOnlySpan<char> input)
        {
            if (input.Length != 3)
                throw new ArgumentException("Invalid abbreviated day of week length");

            // Use bitwise OR with 0x20 for fast case-insensitive comparison
            char c1 = (char)(input[0] | 0x20);
            char c2 = (char)(input[1] | 0x20);
            char c3 = (char)(input[2] | 0x20);

            switch (c1)
            {
                case 'm':
                    if (c2 == 'o' && c3 == 'n') return DayOfWeek.Monday; // mon
                    break;
                case 't':
                    if (c2 == 'u' && c3 == 'e') return DayOfWeek.Tuesday; // tue
                    if (c2 == 'h' && c3 == 'u') return DayOfWeek.Thursday; // thu
                    break;
                case 'w':
                    if (c2 == 'e' && c3 == 'd') return DayOfWeek.Wednesday; // wed
                    break;
                case 'f':
                    if (c2 == 'r' && c3 == 'i') return DayOfWeek.Friday; // fri
                    break;
                case 's':
                    if (c2 == 'a' && c3 == 't') return DayOfWeek.Saturday; // sat
                    if (c2 == 'u' && c3 == 'n') return DayOfWeek.Sunday; // sun
                    break;
            }

            throw new ArgumentException("Unrecognized abbreviated day of week for this locale");
        }


        public override DayOfWeek ParseDayOfWeekFull(ReadOnlySpan<char> input)
        {
            if (input.Length < 6) // Shortest is "friday" (6 chars)
                throw new ArgumentException("Invalid full day of week length");

            // Use bitwise OR with 0x20 for fast case-insensitive comparison
            char firstChar = (char)(input[0] | 0x20);

            switch (firstChar)
            {
                case 'm':
                    // Monday
                    if (input.Length == 6 &&
                        (input[1] | 0x20) == 'o' &&
                        (input[2] | 0x20) == 'n' &&
                        (input[3] | 0x20) == 'd' &&
                        (input[4] | 0x20) == 'a' &&
                        (input[5] | 0x20) == 'y')
                        return DayOfWeek.Monday;
                    break;

                case 't':
                    if (input.Length == 7)
                    {
                        char secondChar = (char)(input[1] | 0x20);
                        if (secondChar == 'u')
                        {
                            // Tuesday
                            if ((input[2] | 0x20) == 'e' &&
                                (input[3] | 0x20) == 's' &&
                                (input[4] | 0x20) == 'd' &&
                                (input[5] | 0x20) == 'a' &&
                                (input[6] | 0x20) == 'y')
                                return DayOfWeek.Tuesday;
                        }
                        else if (secondChar == 'h')
                        {
                            // Thursday
                            if ((input[2] | 0x20) == 'u' &&
                                (input[3] | 0x20) == 'r' &&
                                (input[4] | 0x20) == 's' &&
                                (input[5] | 0x20) == 'd' &&
                                (input[6] | 0x20) == 'a' &&
                                (input[7] | 0x20) == 'y')
                                return DayOfWeek.Thursday;
                        }
                    }
                    else if (input.Length == 8 && (input[1] | 0x20) == 'h')
                    {
                        // Thursday (8 chars)
                        if ((input[2] | 0x20) == 'u' &&
                            (input[3] | 0x20) == 'r' &&
                            (input[4] | 0x20) == 's' &&
                            (input[5] | 0x20) == 'd' &&
                            (input[6] | 0x20) == 'a' &&
                            (input[7] | 0x20) == 'y')
                            return DayOfWeek.Thursday;
                    }

                    break;

                case 'w':
                    // Wednesday
                    if (input.Length == 9 &&
                        (input[1] | 0x20) == 'e' &&
                        (input[2] | 0x20) == 'd' &&
                        (input[3] | 0x20) == 'n' &&
                        (input[4] | 0x20) == 'e' &&
                        (input[5] | 0x20) == 's' &&
                        (input[6] | 0x20) == 'd' &&
                        (input[7] | 0x20) == 'a' &&
                        (input[8] | 0x20) == 'y')
                        return DayOfWeek.Wednesday;
                    break;

                case 'f':
                    // Friday
                    if (input.Length == 6 &&
                        (input[1] | 0x20) == 'r' &&
                        (input[2] | 0x20) == 'i' &&
                        (input[3] | 0x20) == 'd' &&
                        (input[4] | 0x20) == 'a' &&
                        (input[5] | 0x20) == 'y')
                        return DayOfWeek.Friday;
                    break;

                case 's':
                    if (input.Length == 8)
                    {
                        // Saturday
                        if ((input[1] | 0x20) == 'a' &&
                            (input[2] | 0x20) == 't' &&
                            (input[3] | 0x20) == 'u' &&
                            (input[4] | 0x20) == 'r' &&
                            (input[5] | 0x20) == 'd' &&
                            (input[6] | 0x20) == 'a' &&
                            (input[7] | 0x20) == 'y')
                            return DayOfWeek.Saturday;
                    }
                    else if (input.Length == 6)
                    {
                        // Sunday
                        if ((input[1] | 0x20) == 'u' &&
                            (input[2] | 0x20) == 'n' &&
                            (input[3] | 0x20) == 'd' &&
                            (input[4] | 0x20) == 'a' &&
                            (input[5] | 0x20) == 'y')
                            return DayOfWeek.Sunday;
                    }

                    break;
            }

            throw new ArgumentException("Unrecognized full day of week for this locale");
        }


        public override ReadOnlySpan<char> ConsumeDayOfWeek(ref ReadOnlySpan<char> input, ref int inputIndex)
        {
            if (inputIndex >= input.Length)
                throw new FormatException("Unexpected end of input");

            char firstChar = (char)(input[inputIndex] | 0x20); // Fast lowercase conversion

            switch (firstChar)
            {
                case 'm':
                    // Monday
                    if (CheckStringMatch(input, inputIndex, "monday"))
                    {
                        var result = input.Slice(inputIndex, 6);
                        inputIndex += 6;
                        return result;
                    }

                    break;

                case 't':
                    if (inputIndex + 1 < input.Length)
                    {
                        char secondChar = (char)(input[inputIndex + 1] | 0x20);
                        if (secondChar == 'u')
                        {
                            // Tuesday
                            if (CheckStringMatch(input, inputIndex, "tuesday"))
                            {
                                var result = input.Slice(inputIndex, 7);
                                inputIndex += 7;
                                return result;
                            }
                        }
                        else if (secondChar == 'h')
                        {
                            // Thursday
                            if (CheckStringMatch(input, inputIndex, "thursday"))
                            {
                                var result = input.Slice(inputIndex, 8);
                                inputIndex += 8;
                                return result;
                            }
                        }
                    }

                    break;

                case 'w':
                    // Wednesday
                    if (CheckStringMatch(input, inputIndex, "wednesday"))
                    {
                        var result = input.Slice(inputIndex, 9);
                        inputIndex += 9;
                        return result;
                    }

                    break;

                case 'f':
                    // Friday
                    if (CheckStringMatch(input, inputIndex, "friday"))
                    {
                        var result = input.Slice(inputIndex, 6);
                        inputIndex += 6;
                        return result;
                    }

                    break;

                case 's':
                    if (inputIndex + 1 < input.Length)
                    {
                        char secondChar = (char)(input[inputIndex + 1] | 0x20);
                        if (secondChar == 'a')
                        {
                            // Saturday
                            if (CheckStringMatch(input, inputIndex, "saturday"))
                            {
                                var result = input.Slice(inputIndex, 8);
                                inputIndex += 8;
                                return result;
                            }
                        }
                        else if (secondChar == 'u')
                        {
                            // Sunday
                            if (CheckStringMatch(input, inputIndex, "sunday"))
                            {
                                var result = input.Slice(inputIndex, 6);
                                inputIndex += 6;
                                return result;
                            }
                        }
                    }

                    break;
            }

            throw new FormatException("Unrecognized day of week format for this locale");
        }
    }
}