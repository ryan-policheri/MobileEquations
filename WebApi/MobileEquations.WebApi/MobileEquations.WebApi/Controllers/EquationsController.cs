using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileEquations.Model;
using MobileEquations.WebApi.Extensions;
using MobileEquations.WebApi.ModelBinding;

namespace MobileEquations.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquationsController : ControllerBase
    {
        [HttpGet]
        [Route("Ping")]
        public bool Ping() => true;

        //[HttpPost]
        //public string Solve([FromForm] IFormFile file)
        //{
        //    Console.Write("test");
        //    return "Hello";
        //}

        [HttpPost]
        public string Solve([ModelBinder(BinderType = typeof(JsonModelBinder))] Equation equation, IFormFile photo)
        {
            equation.Photo = photo.ToInMemoryFile();
            System.IO.File.WriteAllBytes($"C:\\Users\\Ryan-\\{equation.Photo.FileName}", equation.Photo.bytes);
            return "The solution";;
        }
    }
}
