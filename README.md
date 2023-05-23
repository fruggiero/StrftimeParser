# StrftimeParser

StrftimeParser is a .NET Standard 2.0 library that allows you to work with C-style strftime string formats within .NET.

The project provides a main class called `Strftime` that exposes a static method `Parse`, which allows to convert a 
formatted string to a DateTime object

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

// dateTime now contains the formatted date as a .NET DateTime object

```

## Format Support

At the moment, StrftimeParser supports these string formats:

- `%a` - Abbreviated weekday name *
- `%A` - Full weekday name *
- `%b` - Abbreviated month name *
- `%B` - Full month name *
- `%d` - Day of the month as a zero-padded number
- `%D` - Short MM/DD/YY date, equivalent to %m/%d/%y
- `%e` - Day of the month as a space-padded number
- `%F` - Short YYYY-MM-DD date, equivalent to %Y-%m-%d


\* The specifiers marked with an asterisk (*) are locale-dependent.