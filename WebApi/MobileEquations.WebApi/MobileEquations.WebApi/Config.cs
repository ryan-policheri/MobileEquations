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

            WorkingDirectory = _rawConfig[nameof(WorkingDirectory)]; //TODO: Autobind json to props
            if (String.IsNullOrWhiteSpace(WorkingDirectory)) throw new ArgumentNullException(nameof(WorkingDirectory) + " cannot be empty");
        }

        public string WorkingDirectory { get; }
    }
}
