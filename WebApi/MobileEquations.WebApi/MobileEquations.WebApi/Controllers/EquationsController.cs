using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string _photoPath;

        public EquationsController(Config config)
        {
            _config = config;
            _photoPath = SystemFunctions.CombineDirectoryComponents(_config.WorkingDirectory, "EquationPhotos");
            SystemFunctions.CreateDirectory(_photoPath);
        }

        [HttpGet]
        [Route("Ping")]
        public bool Ping() => true;

        [HttpPost]
        public string Solve([ModelBinder(BinderType = typeof(JsonModelBinder))] Equation equation, IFormFile photo)
        {
            equation.Photo = photo.ToInMemoryFile();
            string path = SystemFunctions.CombineDirectoryComponents(_photoPath, SystemFunctions.GetDateTimeAsFileNameSafeString() + "_" + equation.Photo.FileName);
            System.IO.File.WriteAllBytes(path, equation.Photo.bytes);
            return "The solution to your equation 9 + 4 = 13";
        }
    }
}
