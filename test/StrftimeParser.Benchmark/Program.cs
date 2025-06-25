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
}