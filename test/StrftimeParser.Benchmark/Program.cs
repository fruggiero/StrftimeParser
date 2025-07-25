using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using System.Globalization;
using BenchmarkDotNet.Running;
using StrftimeParser;

var summary = BenchmarkRunner.Run<ParserBenchmark>();

[Config(typeof(Config))]
[MemoryDiagnoser]
public class ParserBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default
                .WithToolchain(InProcessEmitToolchain.Instance)
                .WithIterationCount(15)
                .WithWarmupCount(2));
        }
    }
    
    [Benchmark]
    public DateTime ParseBenchmark()
    {
        return Strftime.Parse("2023-10-01 12:00:00", "%Y-%m-%d %H:%M:%S", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public string ToStringBenchmark()
    {
        return Strftime.ToString(new DateTime(2023, 10, 1, 12, 0, 0), "%Y-%m-%d %H:%M:%S", CultureInfo.InvariantCulture);
    }

    // Benchmarks for individual parsing components to measure optimization impact
    [Benchmark]
    public DateTime ParseDateOnly()
    {
        return Strftime.Parse("2023-12-31", "%Y-%m-%d", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseTimeOnly()
    {
        return Strftime.Parse("23:59:59", "%H:%M:%S", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseMonthIntensive()
    {
        // This will stress test the ParseMonth method specifically
        return Strftime.Parse("12", "%m", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseDayIntensive()
    {
        // This will stress test the ParseDay method specifically
        return Strftime.Parse("31", "%d", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseYearIntensive()
    {
        // This will stress test the ParseYear method specifically
        return Strftime.Parse("2023", "%Y", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseHourIntensive()
    {
        // This will stress test the ParseHour method specifically
        return Strftime.Parse("23", "%H", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseMinuteIntensive()
    {
        // This will stress test the ParseMinute method specifically
        return Strftime.Parse("59", "%M", CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public DateTime ParseSecondIntensive()
    {
        // This will stress test the ParseSecond method specifically
        return Strftime.Parse("59", "%S", CultureInfo.InvariantCulture);
    }

    // Batch parsing to measure cumulative impact
    [Benchmark]
    public DateTime[] ParseBatchDates()
    {
        var dates = new[]
        {
            "2023-01-15", "2023-02-28", "2023-03-31", "2023-04-30", "2023-05-15",
            "2023-06-21", "2023-07-04", "2023-08-25", "2023-09-10", "2023-10-31",
            "2023-11-11", "2023-12-25"
        };
        
        var results = new DateTime[dates.Length];
        for (int i = 0; i < dates.Length; i++)
        {
            results[i] = Strftime.Parse(dates[i], "%Y-%m-%d", CultureInfo.InvariantCulture);
        }
        return results;
    }

    [Benchmark]
    public DateTime[] ParseBatchTimes()
    {
        var times = new[]
        {
            "00:00:00", "01:30:45", "02:15:30", "03:45:15", "04:20:50",
            "05:55:25", "06:10:40", "07:35:55", "08:25:10", "09:50:35",
            "10:15:20", "11:40:45"
        };
        
        var results = new DateTime[times.Length];
        for (int i = 0; i < times.Length; i++)
        {
            results[i] = Strftime.Parse(times[i], "%H:%M:%S", CultureInfo.InvariantCulture);
        }
        return results;
    }

    [Benchmark]
    public DateTime[] ParseBatchDateTime()
    {
        var dateTimes = new[]
        {
            "2023-01-15 08:30:45", "2023-02-28 14:15:30", "2023-03-31 20:45:15",
            "2023-04-30 09:20:50", "2023-05-15 16:55:25", "2023-06-21 12:10:40",
            "2023-07-04 18:35:55", "2023-08-25 07:25:10", "2023-09-10 13:50:35",
            "2023-10-31 19:15:20", "2023-11-11 10:40:45", "2023-12-25 22:05:30"
        };
        
        var results = new DateTime[dateTimes.Length];
        for (int i = 0; i < dateTimes.Length; i++)
        {
            results[i] = Strftime.Parse(dateTimes[i], "%Y-%m-%d %H:%M:%S", CultureInfo.InvariantCulture);
        }
        return results;
    }
}