using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.DependencyInjection;

namespace MobileEquations.Benchmarker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceProvider provider = Bootstrapper.BuildServiceProvider(Bootstrapper.LoadConfiguration());
            BenchmarkerConfig config = provider.GetRequiredService<BenchmarkerConfig>();

            BenchmarkProcessor processor = provider.GetRequiredService<BenchmarkProcessor>();
            IEnumerable<Trial> trials = await processor.RunAsync();
            TrialReporter reporter = new TrialReporter(trials, config.TrialCount);

            ExcelService excelService = provider.GetRequiredService<ExcelService>();
            excelService.ExportTests(reporter);
        }
    }
}
