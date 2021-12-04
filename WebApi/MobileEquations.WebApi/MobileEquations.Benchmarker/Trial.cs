namespace MobileEquations.Benchmarker
{
    public class Trial
    {
        public string FileName { get; set; }

        public string Expression { get; set; }

        public string Answer { get; set; }
        public double PythonRuntime { get; set; }
    }
}
