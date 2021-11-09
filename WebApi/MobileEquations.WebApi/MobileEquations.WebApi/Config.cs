using System.Collections.Generic;
using System.Reflection;
using DotNetCommon.Extensions;
using Microsoft.Extensions.Configuration;

namespace MobileEquations.WebApi
{
    public class Config
    {
        private readonly IConfiguration _rawConfig;

        public Config(IConfiguration rawConfig)
        {
            _rawConfig = rawConfig;

            //Autobinding appsettings data to the strongly-typed properties of this object
            IEnumerable<PropertyInfo> props = typeof(Config).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string rawValue = _rawConfig[prop.Name];
                prop.SetValueWithTypeRespect(this, rawValue);
            }
        }

        public string SolveRequestsDirectory { get; set; }

        public string EquationSolverScript { get; set; }

        public string PythonExecutable { get; set; }
    }
}
