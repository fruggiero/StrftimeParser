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
            if (input.IsEmpty)
                throw new FormatException("Empty month name");

            char firstChar = (char)(input[0] | 0x20);
            switch (firstChar)
            {
                case 'j':
                    if (input.Length == 7 && IsMatchIgnoringCase(input, 0, "january".AsSpan()))
                        return 1;
                    if (input.Length == 4 && IsMatchIgnoringCase(input, 0, "june".AsSpan()))
                        return 6;
                    if (input.Length == 4 && IsMatchIgnoringCase(input, 0, "july".AsSpan()))
                        return 7;
                    break;
                case 'f':
                    if (input.Length == 8 && IsMatchIgnoringCase(input, 0, "february".AsSpan()))
                        return 2;
                    break;
                case 'm':
                    if (input.Length == 5 && IsMatchIgnoringCase(input, 0, "march".AsSpan()))
                        return 3;
                    if (input.Length == 3 && IsMatchIgnoringCase(input, 0, "may".AsSpan()))
                        return 5;
                    break;
                case 'a':
                    if (input.Length == 5 && IsMatchIgnoringCase(input, 0, "april".AsSpan()))
                        return 4;
                    if (input.Length == 6 && IsMatchIgnoringCase(input, 0, "august".AsSpan()))
                        return 8;
                    break;
                case 's':
                    if (input.Length == 9 && IsMatchIgnoringCase(input, 0, "september".AsSpan()))
                        return 9;
                    break;
                case 'o':
                    if (input.Length == 7 && IsMatchIgnoringCase(input, 0, "october".AsSpan()))
                        return 10;
                    break;
                case 'n':
                    if (input.Length == 8 && IsMatchIgnoringCase(input, 0, "november".AsSpan()))
                        return 11;
                    break;
                case 'd':
                    if (input.Length == 8 && IsMatchIgnoringCase(input, 0, "december".AsSpan()))
                        return 12;
                    break;
            }
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
                                if (IsMatchIgnoringCase(input, inputIndex, "january".AsSpan()))
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
                                            if (IsMatchIgnoringCase(input, inputIndex, "july".AsSpan()))
                                            {
                                                var result = input.Slice(inputIndex, 4);
                                                inputIndex += 4;
                                                return result;
                                            }

                                            break;
                                        case 'n':
                                            // June
                                            if (IsMatchIgnoringCase(input, inputIndex, "june".AsSpan()))
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
                    if (IsMatchIgnoringCase(input, inputIndex, "february".AsSpan()))
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
                                if (IsMatchIgnoringCase(input, inputIndex, "march".AsSpan()))
                                {
                                    var result = input.Slice(inputIndex, 5);
                                    inputIndex += 5;
                                    return result;
                                }

                                break;
                            case 'y':
                                // May
                                if (IsMatchIgnoringCase(input, inputIndex, "may".AsSpan()))
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
                                if (IsMatchIgnoringCase(input, inputIndex, "april".AsSpan()))
                                {
                                    var result = input.Slice(inputIndex, 5);
                                    inputIndex += 5;
                                    return result;
                                }

                                break;
                            case 'u':
                                // August
                                if (IsMatchIgnoringCase(input, inputIndex, "august".AsSpan()))
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
                    if (IsMatchIgnoringCase(input, inputIndex, "september".AsSpan()))
                    {
                        var result = input.Slice(inputIndex, 9);
                        inputIndex += 9;
                        return result;
                    }

                    break;

                case 'o':
                    // October
                    if (IsMatchIgnoringCase(input, inputIndex, "october".AsSpan()))
                    {
                        var result = input.Slice(inputIndex, 7);
                        inputIndex += 7;
                        return result;
                    }

                    break;

                case 'n':
                    // November
                    if (IsMatchIgnoringCase(input, inputIndex, "november".AsSpan()))
                    {
                        var result = input.Slice(inputIndex, 8);
                        inputIndex += 8;
                        return result;
                    }

                    break;

                case 'd':
                    // December
                    if (IsMatchIgnoringCase(input, inputIndex, "december".AsSpan()))
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
                    if (IsMatchIgnoringCase(input, inputIndex, "monday".AsSpan()))
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
                            if (IsMatchIgnoringCase(input, inputIndex, "tuesday".AsSpan()))
                            {
                                var result = input.Slice(inputIndex, 7);
                                inputIndex += 7;
                                return result;
                            }
                        }
                        else if (secondChar == 'h')
                        {
                            // Thursday
                            if (IsMatchIgnoringCase(input, inputIndex, "thursday".AsSpan()))
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
                    if (IsMatchIgnoringCase(input, inputIndex, "wednesday".AsSpan()))
                    {
                        var result = input.Slice(inputIndex, 9);
                        inputIndex += 9;
                        return result;
                    }

                    break;

                case 'f':
                    // Friday
                    if (IsMatchIgnoringCase(input, inputIndex, "friday".AsSpan()))
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
                            if (IsMatchIgnoringCase(input, inputIndex, "saturday".AsSpan()))
                            {
                                var result = input.Slice(inputIndex, 8);
                                inputIndex += 8;
                                return result;
                            }
                        }
                        else if (secondChar == 'u')
                        {
                            // Sunday
                            if (IsMatchIgnoringCase(input, inputIndex, "sunday".AsSpan()))
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