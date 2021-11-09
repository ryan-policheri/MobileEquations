using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCommon.Constants;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Logging;
using MobileEquations.Model;
using MobileEquations.WebApi.Extensions;
using MobileEquations.WebApi.ModelBinding;

namespace MobileEquations.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquationsController : ControllerBase
    {
        private readonly Config _config;
        private readonly string _solveRequestsPath;
        private readonly ILogger<EquationsController> _logger;

        public EquationsController(Config config, ILogger<EquationsController> logger)
        {
            _config = config;
            _solveRequestsPath = _config.SolveRequestsDirectory;
            _logger = logger;
            SystemFunctions.CreateDirectory(_solveRequestsPath);
        }

        [HttpGet]
        [Route("Ping")]
        public bool Ping() => true;

        [HttpGet]
        [Route("PingPython")]
        public bool PingPython()
        {
            try
            {
                string command = $"{_config.EquationSolverScript.Quotify()} {inputFile.Quotify()} {outputFile.Quotify()}";
                _logger.LogInformation($"Calling {_config.PythonExecutable.Quotify()}");
                SystemFunctions.RunCustomProcess($"{_config.PythonExecutable.Quotify()}", null);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }

        [HttpPost]
        public Equation Solve([ModelBinder(BinderType = typeof(JsonModelBinder))] Equation equation, IFormFile photo)
        {
            try
            {
                equation.Photo = photo.ToInMemoryFile();
                string requestDirectory = CreateRequestDirectory();
                string photoPath = SystemFunctions.CombineDirectoryComponents(requestDirectory, equation.Photo.FileName);
                string inputFile = SystemFunctions.CombineDirectoryComponents(requestDirectory, "Input.json");
                string outputFile = SystemFunctions.CombineDirectoryComponents(requestDirectory, "Output.json");

                SystemFunctions.CreateFile(photoPath, equation.Photo.Bytes);
                SystemFunctions.CreateFile(inputFile, equation.ToJson());

                string command = $"{_config.EquationSolverScript.Quotify()} {inputFile.Quotify()} {outputFile.Quotify()}";
                _logger.LogInformation($"Calling {_config.PythonExecutable.Quotify()} with the following command: {command}");
                SystemFunctions.RunCustomProcess($"{_config.PythonExecutable.Quotify()}", command);
                string output = SystemFunctions.ReadAllText(outputFile);
                equation.ProcessedEquation = output.ConvertJsonToObject<ProcessedEquation>(JsonSerializationOptions.CaseInsensitive);

                return equation;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }

        private string CreateRequestDirectory()
        {
            string uniqueId = SystemFunctions.GetDateTimeAsFileNameSafeString() + Guid.NewGuid().ToString().Substring(0, 4);
            string requestDirectory = SystemFunctions.CombineDirectoryComponents(_solveRequestsPath, uniqueId);
            SystemFunctions.CreateDirectory(requestDirectory);
            return requestDirectory;
        }
    }
}
