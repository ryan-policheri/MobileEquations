using System;
using Microsoft.Extensions.Configuration;

namespace MobileEquations.WebApi
{
    public class Config
    {
        private readonly IConfiguration _rawConfig;

        public Config(IConfiguration rawConfig)
        {
            _rawConfig = rawConfig;

            FileDirectory = _rawConfig[nameof(FileDirectory)]; //TODO: Autobind json to props
            if (String.IsNullOrWhiteSpace(FileDirectory)) throw new ArgumentNullException(nameof(FileDirectory) + " cannot be empty");
        }

        public string FileDirectory { get; }
    }
}
