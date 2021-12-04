using System;
using Microsoft.Extensions.DependencyInjection;

namespace MobileEquations.Benchmarker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider provider = Bootstrapper.BuildServiceProvider(Bootstrapper.LoadConfiguration());
            BenchmarkProcessor processor = provider.GetRequiredService<BenchmarkProcessor>();

            processor.Run();
        }
    }
}
