# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Added support for flexible day of month parsing (handles both zero-padded and space-padded formats)
- Added BenchmarkDotNet project for performance testing
- Added comprehensive POSIX compatibility test suite (138 new tests)
- Added `TryParse` methods with proper .NET conventions (returns bool, out parameter for result)
- Added `ReadOnlySpan<char>` overloads for `Parse` and `TryParse` methods for zero-allocation scenarios
- Added null argument validation with `ArgumentNullException` for all public methods
- Added comprehensive XML documentation for all public API methods
- Added `ToStrftimeString` extension methods for DateTime objects

### Changed
- **PERFORMANCE**: Significant performance improvements in parsing operations
- **PERFORMANCE**: Optimized `ToDayOfWeek` method from O(n) loop to O(1) arithmetic calculation
- Refactored `ToString` method to use while loop instead of for loop with internal index modification
- Improved exception messages for better error clarity

### Fixed
- Fixed `IndexOutOfRangeException` crash in `ParseDayOfWeekFull` when parsing 7-character strings starting with "Th"
- Fixed incorrect parsing when "February" appears in date strings with additional components
- Fixed case-sensitivity issues in `GenericFormatter` - month and day name parsing is now case-insensitive for all cultures (POSIX strptime compliance)

### Internal
- Replaced string operations with ReadOnlySpan<char> for better memory efficiency
- Implemented manual parsing for numeric values instead of using int.Parse
- Added ConcurrentDictionary-based formatter caching
- Added System.Memory package dependency
- Refactored all Consume* methods to use ReadOnlySpan<char>
- Updated project sdk to .NET 8.0 (was .NET 6.0)
- Added `EqualsIgnoreCase` helper method for case-insensitive span comparison
- Refactored Parse method to use while loop for cleaner control flow
- Removed some unused structs

## [Previous Versions]

_No previous versions documented yet._