using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCommon.Logging.File;
using MobileEquations.Model;
using MobileEquations.Services;

namespace MobileEquations.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ApiConfig config = new ApiConfig(Configuration);
            services.AddSingleton<BaseConfig>(config);
            services.AddSingleton<ApiConfig>(config);

            string fileDirectory = config.FileLoggerDirectory;
            string fileName = $"MobileEquationsWebApiLog_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
            FileLoggerConfig fileLoggerConfig = new FileLoggerConfig(fileDirectory, fileName);
            FileLoggerProvider fileLoggerProvider = new FileLoggerProvider(fileLoggerConfig);

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddProvider(fileLoggerProvider);
            });

            services.AddSingleton<EquationSolverService>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MobileEquations.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MobileEquations.WebApi v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
