# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Added support for flexible day of month parsing (handles both zero-padded and space-padded formats)
- Added BenchmarkDotNet project for performance testing

### Changed
- **PERFORMANCE**: Significant performance improvements in parsing operations
- Improved exception messages for better error clarity

### Fixed
- Fixed incorrect parsing when "February" appears in date strings with additional components
- Fixed case-sensitivity issues in culture-specific parsing

### Internal
- Replaced string operations with ReadOnlySpan<char> for better memory efficiency
- Implemented manual parsing for numeric values instead of using int.Parse
- Added ConcurrentDictionary-based formatter caching
- Added System.Memory package dependency
- Refactored all Consume* methods to use ReadOnlySpan<char>
- Updated project sdk to .NET 8.0 (was .NET 6.0)

## [Previous Versions]

_No previous versions documented yet._