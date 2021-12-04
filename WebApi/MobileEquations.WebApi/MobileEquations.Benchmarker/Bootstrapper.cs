using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MobileEquations.Benchmarker
{
    public class Bootstrapper
    {
        public static IConfiguration LoadConfiguration()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfiguration rawConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .Build();

            return rawConfig;
        }

        public static IServiceProvider BuildServiceProvider(IConfiguration _config)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<BenchmarkerConfig>(new BenchmarkerConfig(_config));
            services.AddTransient<ExcelService>();
            services.AddTransient<BenchmarkProcessor>();

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });


            return services.BuildServiceProvider();
        }
    }
}
