using System.Collections.Generic;
using System.Reflection;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Configuration;
using MobileEquations.Model;

namespace MobileEquations.WebApi
{
    public class ApiConfig : BaseConfig
    {
        private readonly IConfiguration _rawConfig;

        public ApiConfig(IConfiguration rawConfig)
        {
            _rawConfig = rawConfig;

            //Autobinding appsettings data to the strongly-typed properties of this object
            IEnumerable<PropertyInfo> props = typeof(BaseConfig).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string rawValue = _rawConfig[prop.Name];
                prop.SetValueWithTypeRespect(this, rawValue);
            }
            if (User != null) SystemFunctions.User = User;
        }
    }
}
