using System.IO;
using System.Linq;
using MobileEquations.Model;

namespace MobileEquations.Benchmarker
{
    public static class Extensions
    {
        public static InMemoryFile ToInMemoryFile(this FileInfo source)
        {
            InMemoryFile file = new InMemoryFile();
            file.FileName = source.Name;
            file.Extension = source.Name.Split(".").Last();

            using (Stream stream = source.OpenRead())
            {
                byte[] bytes = new byte[source.Length];
                stream.Read(bytes, 0, (int)source.Length);
                file.Bytes = bytes;
            }

            return file;
        }
    }
}