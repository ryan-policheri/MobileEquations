using System.Text.Json.Serialization;

namespace MobileEquations.Model
{
    public class InMemoryFile
    {
        public string FileName { get; set; }

        public string Extension { get; set; }

        [JsonIgnore]
        public byte[] Bytes { get; set; }
    }
}