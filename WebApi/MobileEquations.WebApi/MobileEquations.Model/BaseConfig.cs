using System.IO;

namespace MobileEquations.Model
{
    public class BaseConfig
    {
        public string FileLoggerDirectory { get; set; }

        public string SolveRequestsDirectory { get; set; }

        public string EquationSolverScript { get; set; }

        public string EquationSolverOwningDirectory => new FileInfo(EquationSolverScript).DirectoryName;
        public bool EquationSolverIsPackaged { get; set; }

        public string PythonExecutable { get; set; }

        public string User { get; set; }
    }
}
