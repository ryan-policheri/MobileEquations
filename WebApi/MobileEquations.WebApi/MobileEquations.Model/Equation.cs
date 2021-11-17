namespace MobileEquations.Model
{
    public class Equation
    {
        public ClientInfo ClientInfo { get; set; }

        public InMemoryFile Photo { get; set; }

        public ProcessedEquation ProcessedEquation { get; set; }
    }
}