namespace MobileEquations.Model
{
    public class BaseConfig
    {
        public string FileLoggerDirectory { get; set; }

        public string SolveRequestsDirectory { get; set; }

        public string EquationSolverScript { get; set; }

        public bool EquationSolverIsPackaged { get; set; }

        public string PythonExecutable { get; set; }
    }
}
