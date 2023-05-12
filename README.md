# StrftimeParser

StrftimeParser is a .NET Standard 2.0 library that allows you to convert the C-style strftime string format to a DateTime format compatible with C#.

The date and time string formatting functionality in C is based on the strftime function, which allows you to format a date object into a custom string.
However, when working in C#, the date and time string format is different from that of C, and thus the formatted strings in C cannot be used directly with the DateTime type in C#.

The project provides a main class called `Strftime` that exposes a static method `Parse`

## Usage

To use StrftimeParser in your .NET project, follow these simple steps:

1. Add the NuGet package StrftimeParser to your .NET project.

2. Import the StrftimeParser namespace in your code file.

3. Call the static method Parse of the Strftime class and pass a string in the C-style strftime format as a parameter, plus the format used.

4. Use the DateTime object returned by the Parse method to manipulate the formatted date.

Here an example of code:

```
using StrftimeParser;

string strftimeString = "%Y-%m-%d %H:%M:%S";
DateTime dateTime = Strftime.Parse(strftimeString);

// dateTime now contains the formatted date in the C# format

```

## Format Support

At the moment, StrftimeParser supports these string formats:

- `%a` - abbreviated weekday name
- `%A` - full weekday name
- `%d` - day of the month as a zero-padded number
- `%e` - day of the month as a space-padded number