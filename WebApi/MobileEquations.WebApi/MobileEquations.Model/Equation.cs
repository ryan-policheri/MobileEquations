using System;

namespace MobileEquations.Model
{
    public class Equation
    {
        public string Client { get; set; }

        public InMemoryFile Photo { get; set; }

        public ProcessedEquation ProcessedEquation { get; set; }
    }
}