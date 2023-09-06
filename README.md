[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/L4L1NVECU)

# StrftimeParser

StrftimeParser is a .NET Standard 2.0 library that allows you to work with C-style strftime/strptime string formats within .NET.

The project provides a main class called `Strftime` that exposes a static method `Parse`, and a static method `ToString`.

The `Parse` method is the equivalent of the `strptime` in `time.h` of C language, as it allows to obtain a DateTime object given a format specifier and a formatted date string.

The `ToString` method is the equivalent of the `strftime` in `time.h` of C language, as it allows to obtain a formatted date string, given a format specifier and a DateTime object

## Usage

To use StrftimeParser in your .NET project, follow these simple steps:

1. Add the NuGet package StrftimeParser to your .NET project.

2. Import the StrftimeParser namespace in your code file.

3. Now call the `Parse` or the `ToString` method


### Here an example of `Parse`:

```
using StrftimeParser;

string strftimeString = "2001-08-23 14:55:02";
string formatSpecifier = "%Y-%m-%d %H:%M:%S";

DateTime dateTime = Strftime.Parse(strftimeString, formatSpecifier);

// dateTime now contains the formatted date as a .NET DateTime object

```

### Here an example of `ToString`:

```
using StrftimeParser;

var dt = new DateTime(1970, 1, 2, 3, 4, 5);
var format = "%c";

var res = Strftime.ToString(dt, format);

// res now contains the formatted date string
```

## How to use a specified CultureInfo when parsing

To execute the parsing with a specific Culture, you can pass
a CultureInfo instance to the `Parse` method:

```
DateTime dateTime = Strftime.Parse(strftimeString, formatSpecifier, CultureInfo.GetCultureInfo("it-IT"));

// dateTime now contains the formatted date as a .NET DateTime object
```

## How to use a specified CultureInfo when converting to string

To execute the parsing with a specific Culture, you can pass
a CultureInfo instance to the `ToString` method:

```
var result = Strftime.ToString(dt, formatSpecifier, CultureInfo.GetCultureInfo("it-IT"));

// result now contains the formatted string
```

## Compatibility

At the moment, StrftimeParser supports these format specifiers:

- `%a` - Abbreviated weekday name *
- `%A` - Full weekday name *
- `%b` - Abbreviated month name *
- `%B` - Full month name *
- `%d` - Day of the month as a zero-padded number
- `%D` - Short MM/DD/YY date, equivalent to %m/%d/%y
- `%e` - Day of the month as a space-padded number
- `%F` - Short YYYY-MM-DD date, equivalent to %Y-%m-%d
- `%c` - Date and time representation
- `%I` - Hour in 12h format (01-12)
- `%p` - AM or PM designation
- `%H` - Hour in 24h format (00-23)
- `%j` - Day of the year (001-366)
- `%m` - Month as a decimal number (01-12)
- `%M` - Minute (00-59)
- `%n` - New-line character ('\n')
- `%t` - Horizontal-tab character ('\t')
- `%S` - Second (00-61)
- `%T` - ISO 8601 time format (HH:MM:SS), equivalent to %H:%M:%S
- `%u` - ISO 8601 weekday as number with Monday as 1 (1-7)
- `%w` - Weekday as a decimal number with Sunday as 0 (0-6)
- `%y` - Year, last two digits (00-99)
- `%Y` - Year

\* The specifiers marked with an asterisk (*) are locale-dependent.
