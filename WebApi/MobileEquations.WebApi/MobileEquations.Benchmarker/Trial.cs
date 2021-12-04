namespace MobileEquations.Benchmarker
{
    public class Trial
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string Expression { get; set; }

        public string Answer { get; set; }

        public double PythonSystemRuntimeInMilliseconds { get; set; }

        public long PythonDotNetRuntimeInMilliseconds { get; set; }

        public long DotNetServiceRuntimeInMilliseconds { get; set; }

        public long ApiRuntimeInMilliseconds { get; set; }
    }
}
