using System.IO;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

using Benchmarks.AM;

namespace Benchmarks
{
    class Program
    {
        static void Main()
        {
            ManualConfig config = new ManualConfig
            {
                ArtifactsPath = Path.GetTempPath()
            };
            config.Add(new ConsoleLogger());
            config.Add(TargetMethodColumn.Method);
            config.Add(StatisticColumn.Mean);
            config.Add(StatisticColumn.Error);
            config.Add(StatisticColumn.StdDev);

            BenchmarkRunner.Run<FastNumberBenchmark>(config);
        }
    }
}
