using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace MobileEquations.Benchmarker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider provider = Bootstrapper.BuildServiceProvider(Bootstrapper.LoadConfiguration());
            BenchmarkProcessor processor = provider.GetRequiredService<BenchmarkProcessor>();

            IEnumerable<Trial> trials = processor.Run();
            TrialReporter reporter = new TrialReporter(trials);
        }
    }
}
