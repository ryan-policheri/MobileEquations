using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using MobileEquations.Model;

namespace MobileEquations.Benchmarker
{
    public class BenchmarkerConfig : BaseConfig
    {
        private readonly IConfiguration _rawConfig;

        public BenchmarkerConfig(IConfiguration config)
        {
            _rawConfig = config;

            //Autobinding appsettings data to the strongly-typed properties of this object
            IEnumerable<PropertyInfo> props = typeof(BenchmarkerConfig).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string rawValue = _rawConfig[prop.Name];
                prop.SetValueWithTypeRespect(this, rawValue);
            }

            SolveRequestsDirectory = this.BenchmarkDatasetDirectory;
        }

        public string BenchmarkDatasetDirectory { get; set; }

        public int TrialCount { get; set; }

        public string ApiBaseUrl { get; set; }

        public string LedgerPath => SystemFunctions.CombineDirectoryComponents(BenchmarkDatasetDirectory, "ledger.csv");
    }
}
