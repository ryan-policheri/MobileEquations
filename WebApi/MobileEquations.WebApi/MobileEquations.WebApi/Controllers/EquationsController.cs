using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Logging;
using MobileEquations.Model;
using MobileEquations.WebApi.Extensions;
using MobileEquations.WebApi.ModelBinding;
using MobileEquations.Services;

namespace MobileEquations.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquationsController : ControllerBase
    {
        private readonly ApiConfig _config;
        private readonly EquationSolverService _equationService;
        private readonly ILogger<EquationsController> _logger;

        public EquationsController(ApiConfig config, EquationSolverService equationService, ILogger<EquationsController> logger)
        {
            _config = config;
            _equationService = equationService;
            _logger = logger;
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
                _logger.LogInformation($"Calling {_config.PythonExecutable.Quotify()}");
                SystemFunctions.RunCustomProcess($"{_config.PythonExecutable.Quotify()}", "--version");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }

        [HttpPost("TestSimplePost")]
        public Equation TestPost([FromBody] Equation equation) //This is just here so i can test a normal post (I.E. one without a file)
        {
            equation.ProcessedEquation = new ProcessedEquation
            {
                Equation = "Foo",
                Solution = "bar",
                SolvedEquation = "Foobar",
                LaTeX = "@FOOBAR"
            };

            return equation;
        }

        [HttpPost]
        public Equation Solve([ModelBinder(BinderType = typeof(JsonFormFieldToModelBinder))] Equation equation, IFormFile file)
        {
            try
            {
                equation.Photo = file.ToInMemoryFile();
                string requestDirectory = CreateRequestDirectory();
                return _equationService.SolveEquation(requestDirectory, equation);

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
            string requestDirectory = SystemFunctions.CombineDirectoryComponents(_config.SolveRequestsDirectory, uniqueId);
            SystemFunctions.CreateDirectory(requestDirectory);
            return requestDirectory;
        }
    }
}
