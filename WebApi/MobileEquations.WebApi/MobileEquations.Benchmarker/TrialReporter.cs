using System.Collections.Generic;
using System.Data;
using System.Linq;
using DotNetCommon.Extensions;

namespace MobileEquations.Benchmarker
{
    public class TrialReporter
    {
        public TrialReporter(IEnumerable<Trial> trials, int runsPerTrialCount)
        {
            Trials = trials;
            RunsPerTrialCount = runsPerTrialCount;
        }

        public IEnumerable<Trial> Trials { get; }

        public int RunsPerTrialCount { get; }


        public TableWithMeta GetAllData()
        {
            TableWithMeta tableWithMeta = BuildTable(Trials.OrderBy(x => x.FileName));
            tableWithMeta.Header = $"All Trial Data ({RunsPerTrialCount} Runs per File)";
            return tableWithMeta;
        }

        public TableWithMeta GetAveragedByFileData()
        {
            ICollection<Trial> trials = new List<Trial>();
            foreach (string fileName in GetFileNames())
            {
                trials.Add(GetAverageForTrialSet(fileName));
            }
            TableWithMeta tableWithMeta = BuildTable(trials.OrderBy(x => x.FileName));
            tableWithMeta.Header = $"Averaged By File Trial Data ({RunsPerTrialCount} Runs per File)";
            return tableWithMeta;
        }

        public TableWithMeta GetAllDataAveraged()
        {
            ICollection<Trial> trials = new List<Trial>();
            trials.Add(GetOverallAverage());
            TableWithMeta tableWithMeta = BuildTable(trials.OrderBy(x => x.FileName));
            tableWithMeta.Header = $"Averaged Across All Trials ({Trials.Count()} Total Runs)";
            return tableWithMeta;
        }

        private IEnumerable<string> GetFileNames()
        {
            return Trials.Select(x => x.FileName).Distinct();
        }

        private IEnumerable<Trial> GetTrialsByFileName(string fileName)
        {
            return Trials.Where(x => x.FileName == fileName);
        }

        private Trial GetAverageForTrialSet(string fileName)
        {
            var trials = GetTrialsByFileName(fileName);
            Trial averagedTrial = trials.First().Copy();

            averagedTrial.PythonSystemRuntimeInMilliseconds = (long)trials.Select(x => x.PythonSystemRuntimeInMilliseconds).Average();
            averagedTrial.PythonDotNetRuntimeInMilliseconds = (long)trials.Select(x => x.PythonDotNetRuntimeInMilliseconds).Average();
            averagedTrial.DotNetServiceRuntimeInMilliseconds = (long)trials.Select(x => x.DotNetServiceRuntimeInMilliseconds).Average();
            averagedTrial.ApiRuntimeInMilliseconds = (long)trials.Select(x => x.ApiRuntimeInMilliseconds).Average();

            return averagedTrial;
        }

        private Trial GetOverallAverage()
        {
            var trials = Trials;
            Trial averagedTrial = trials.First().Copy();

            averagedTrial.PythonSystemRuntimeInMilliseconds = (long)trials.Select(x => x.PythonSystemRuntimeInMilliseconds).Average();
            averagedTrial.PythonDotNetRuntimeInMilliseconds = (long)trials.Select(x => x.PythonDotNetRuntimeInMilliseconds).Average();
            averagedTrial.DotNetServiceRuntimeInMilliseconds = (long)trials.Select(x => x.DotNetServiceRuntimeInMilliseconds).Average();
            averagedTrial.ApiRuntimeInMilliseconds = (long)trials.Select(x => x.ApiRuntimeInMilliseconds).Average();

            averagedTrial.FileName = "All Files";

            return averagedTrial;
        }

        private TableWithMeta BuildTable(IEnumerable<Trial> trials)
        {
            TableWithMeta tableWithMeta = new TableWithMeta();
            DataTable table = new DataTable();
            table.Columns.Add("File Name");
            table.Columns.Add("Runtime (ms)");
            table.Columns.Add("Accuracy");

            foreach (Trial trial in Trials)
            {
                DataRow row = table.NewRow();
                row["File Name"] = trial.FileName;
                row["Python Runtime (ms) (OS Measured)"] = trial.PythonSystemRuntimeInMilliseconds;
                row["Python Runtime (ms) (DotNet Measured)"] = trial.PythonDotNetRuntimeInMilliseconds;
                row["DotNet Service Runtime (ms)"] = trial.DotNetServiceRuntimeInMilliseconds;
                row["Api Service Runtime (ms)"] = trial.ApiRuntimeInMilliseconds;
                table.Rows.Add(row);
            }

            tableWithMeta.Table = table;
            return tableWithMeta;
        }
    }
}