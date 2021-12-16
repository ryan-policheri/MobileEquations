using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using DotNetCommon;
using DotNetCommon.Constants;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Logging;
using MobileEquations.Model;
using MobileEquations.Services;

namespace MobileEquations.Benchmarker
{
    public class BenchmarkProcessor
    {
        private readonly BenchmarkerConfig _config;
        private readonly EquationSolverService _equationService;
        private readonly MobileEquationsClient _client;
        private readonly ILogger<BenchmarkProcessor> _logger;

        public BenchmarkProcessor(BenchmarkerConfig config, EquationSolverService equationService, MobileEquationsClient client, ILogger<BenchmarkProcessor> logger)
        {
            _config = config;
            _equationService = equationService;
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<Trial>> RunAsync()
        {
            IEnumerable<Trial> trials = GetTrials();

            foreach (Trial trial in trials)
            {
                trial.FilePath = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, trial.FileName);
                await ExecuteThroughApi(trial);
            }
            foreach (Trial trial in trials)
            {
                trial.FilePath = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, trial.FileName);
                ExecuteThroughDotNet(trial);
            }
            foreach (Trial trial in trials)
            {
                trial.FilePath = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, trial.FileName);
                ExecuteThroughPython(trial);
            }

            return trials;
        }

        private void ExecuteThroughPython(Trial trial)
        {
            string imageFile = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, trial.FileName);
            string resultFile = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, "Result.json");
            ICollection<string> args = new List<string>() { _config.EquationSolverScript, imageFile, resultFile };
            if (!_config.EquationSolverIsPackaged) args = args.Prepend(_config.PythonExecutable).ToList(); //Executing through python, add python as the first argument
            _logger.LogInformation($"Executing the following args as a system process: {args.ToDelimitedList(' ')}");
            ProcessStats stats = null;
            long runtime = ExecuteWithTiming(() => stats = SystemFunctions.RunSystemProcess(args.ToArray(), _config.EquationSolverOwningDirectory));
            trial.PythonSystemRuntimeInMilliseconds = stats != null ? stats.MillisecondsEllapsed : runtime;
            trial.PythonDotNetRuntimeInMilliseconds = runtime;

            if (File.Exists(resultFile))
            {
                string output = SystemFunctions.ReadAllText(resultFile);
                ProcessedEquation processed = output.ConvertJsonToObject<ProcessedEquation>(JsonSerializationOptions.CaseInsensitive);
                trial.ActualResult = processed;
            }
            trial.InitializeAccuracy();

            SystemFunctions.DeleteFile(SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, "Result.json"));
        }

        private void ExecuteThroughDotNet(Trial trial)
        {
            Equation equation = new Equation();
            FileInfo info = new FileInfo(trial.FilePath);
            equation.Photo = info.ToInMemoryFile();
            string tempDir = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, "temp");
            SystemFunctions.CreateDirectory(tempDir);
            trial.DotNetServiceRuntimeInMilliseconds = ExecuteWithTiming(() => _equationService.SolveEquation(tempDir, equation));
            trial.ActualResult = equation.ProcessedEquation;
            trial.InitializeAccuracy();
            SystemFunctions.DeleteDirectory(tempDir);
        }

        private async Task ExecuteThroughApi(Trial trial)
        {
            Equation equation = new Equation();
            FileInfo info = new FileInfo(trial.FilePath);
            equation.Photo = info.ToInMemoryFile();
            Equation solvedEquation = null;
            trial.ApiRuntimeInMilliseconds = await ExecuteWithTimingAsync(async () => solvedEquation = await _client.SolveEquation(equation, trial.FilePath));
            trial.ActualResult = solvedEquation?.ProcessedEquation;
            trial.InitializeAccuracy();
        }

        private long ExecuteWithTiming(Action action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try { action(); }
            catch (Exception ex) { /*EAT*/ }
            finally { watch.Stop(); }
            return watch.ElapsedMilliseconds;
        }

        private async Task<long> ExecuteWithTimingAsync(Func<Task> action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try { await action(); }
            catch (Exception e) { /*EAT*/ }
            finally { watch.Stop(); }
            return watch.ElapsedMilliseconds;
        }

        private IEnumerable<Trial> GetTrials()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            TextReader reader = new StreamReader(_config.LedgerPath);
            CsvReader csvReader = new CsvReader(reader, config);
            List<Trial> trials = csvReader.GetRecords<Trial>().ToList();

            List<Trial> duplicatedTrials = new List<Trial>();

            for (int i = 1; i < _config.TrialCount; i++)
            {
                foreach (Trial trial in trials)
                {
                    Trial dup = trial.Copy();
                    duplicatedTrials.Add(dup);
                }
            }

            trials.AddRange(duplicatedTrials);
            return trials;
        }
    }
}
