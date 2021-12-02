using System.Collections.Generic;
using System.Linq;
using DotNetCommon.Constants;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Logging;
using MobileEquations.Model;

namespace MobileEquations.Services
{
    public class EquationSolverService
    {
        private readonly BaseConfig _config;
        private readonly ILogger<EquationSolverService> _logger;

        public EquationSolverService(BaseConfig config, ILogger<EquationSolverService> logger)
        {
            _config = config;
            _logger = logger;
            SystemFunctions.CreateDirectory(_config.SolveRequestsDirectory);
        }

        public Equation SolveEquation(string workingDirectory, Equation equation)
        {
            string photoFilePath = SystemFunctions.CombineDirectoryComponents(workingDirectory, equation.Photo.FileName);
            string outputFilePath = SystemFunctions.CombineDirectoryComponents(workingDirectory, "Output.json");

            SystemFunctions.CreateFile(photoFilePath, equation.Photo.Bytes);

            ICollection<string> args = new List<string>() { _config.EquationSolverScript, photoFilePath, outputFilePath };
            if (!_config.EquationSolverIsPackaged) args = args.Prepend(_config.PythonExecutable).ToList(); //Executing through python, add python as the first argument
            _logger.LogInformation($"Executing the following args as a system process: {args.ToDelimitedList(' ')}");
            SystemFunctions.RunSystemProcess(args.ToArray());

            string output = SystemFunctions.ReadAllText(outputFilePath);
            equation.ProcessedEquation = output.ConvertJsonToObject<ProcessedEquation>(JsonSerializationOptions.CaseInsensitive);

            return equation;
        }
    }
}
