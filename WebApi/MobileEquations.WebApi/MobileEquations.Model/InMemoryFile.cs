using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileEquations.Model
{
    public class InMemoryFile
    {
        public string FileName { get; set; }

        public string Extension { get; set; }

        public byte[] bytes { get; set; }
    }
}