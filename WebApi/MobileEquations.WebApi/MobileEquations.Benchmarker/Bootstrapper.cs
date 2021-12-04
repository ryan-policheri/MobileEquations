using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MobileEquations.Model;
using MobileEquations.Services;

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

            BenchmarkerConfig config = new BenchmarkerConfig(_config);
            services.AddSingleton<BenchmarkerConfig>(config);
            services.AddSingleton<BaseConfig>(config);
            services.AddTransient<EquationSolverService>();
            services.AddTransient<ExcelService>();
            services.AddTransient<BenchmarkProcessor>();

            services.AddHttpClient("MobileEquationsClient", c =>
            {
                string baseAddress = config.ApiBaseUrl.TrimEnd('/');
                c.BaseAddress = new Uri(baseAddress);
            });

            services.AddTransient<MobileEquationsClient>(x => new MobileEquationsClient(x.GetRequiredService<IHttpClientFactory>().CreateClient("MobileEquationsClient")));

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            return services.BuildServiceProvider();
        }
    }
}
