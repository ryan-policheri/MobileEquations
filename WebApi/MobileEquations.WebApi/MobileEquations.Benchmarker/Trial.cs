namespace MobileEquations.Benchmarker
{
    public class Trial
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string Expression { get; set; }

        public string Answer { get; set; }

        public double PythonRuntimeSystem { get; set; }

        public long PythonRuntimeDotNet { get; set; }

        public long DotNetRuntime { get; set; }

        public long ApiRuntimeInMilliseconds { get; set; }
    }
}
