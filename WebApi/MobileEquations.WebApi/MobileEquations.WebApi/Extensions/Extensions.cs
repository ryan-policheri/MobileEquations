using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MobileEquations.Model;

namespace MobileEquations.WebApi.Extensions
{
    public static class Extensions
    {
        public static InMemoryFile ToInMemoryFile(this IFormFile source)
        {
            InMemoryFile file = new InMemoryFile();
            file.FileName = source.FileName;
            file.Extension = source.FileName.Split(".").Last();

            using (Stream stream = source.OpenReadStream())
            {
                byte[] bytes = new byte[source.Length];
                stream.Read(bytes, 0, (int)source.Length);
                file.Bytes = bytes;
            }

            return file;
        }
    }
}
