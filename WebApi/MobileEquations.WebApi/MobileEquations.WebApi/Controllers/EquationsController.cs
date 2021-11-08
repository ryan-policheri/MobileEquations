using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
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

        public EquationsController(Config config)
        {
            _config = config;
            _solveRequestsPath = SystemFunctions.CombineDirectoryComponents(_config.FileDirectory, "EquationSolveRequests");
            SystemFunctions.CreateDirectory(_solveRequestsPath);
        }

        [HttpGet]
        [Route("Ping")]
        public bool Ping() => true;

        [HttpPost]
        public string Solve([ModelBinder(BinderType = typeof(JsonModelBinder))] Equation equation, IFormFile photo)
        {
            equation.Photo = photo.ToInMemoryFile();
            string requestDirectory = CreateRequestDirectory();
            string photoPath = SystemFunctions.CombineDirectoryComponents(requestDirectory, equation.Photo.FileName);
            string inputFile = SystemFunctions.CombineDirectoryComponents(requestDirectory, "Input.json");
            string outputFile = SystemFunctions.CombineDirectoryComponents(requestDirectory, "Output.json");

            SystemFunctions.CreateFile(photoPath, equation.Photo.Bytes);
            SystemFunctions.CreateFile(inputFile, equation.ToJson());

            string command = "C:\\Users\\Ryan-\\source\\repos\\MobileEquations\\EquationSolver\\EquationSolver.py \"C:\\Users\\Ryan-\\EquationPhotos\\test.json\" \"C:\\Users\\Ryan-\\EquationPhotos\\testout.json\"";
            SystemFunctions.RunCustomProcess("C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\Python36_64\\python", command);
            return "The solution to your equation 9 + 4 = 13";
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
