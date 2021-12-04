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
        private readonly ExcelService _excelService;
        private readonly ILogger<BenchmarkProcessor> _logger;

        public BenchmarkProcessor(BenchmarkerConfig config, EquationSolverService equationService, MobileEquationsClient client, ExcelService excelService, ILogger<BenchmarkProcessor> logger)
        {
            _config = config;
            _equationService = equationService;
            _client = client;
            _excelService = excelService;
            _logger = logger;
        }

        public void Run()
        {
            IEnumerable<Trial> trials = GetTrials();

            foreach (Trial trial in trials)
            {
                trial.FilePath = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, trial.FileName);
                ExecuteThroughPython(trial);
                ExecuteThroughDotNet(trial);
                ExecuteThroughApi(trial);
            }

            Console.Write("break");
        }

        private void ExecuteThroughPython(Trial trial)
        {
            ICollection<string> args = new List<string>() { _config.EquationSolverScript, trial.FileName, "Result.json" };
            if (!_config.EquationSolverIsPackaged) args = args.Prepend(_config.PythonExecutable).ToList(); //Executing through python, add python as the first argument
            _logger.LogInformation($"Executing the following args as a system process: {args.ToDelimitedList(' ')}");
            ProcessStats stats = null;
            long runtime = ExecuteWithTiming(() => stats = SystemFunctions.RunSystemProcess(args.ToArray(), _config.BenchmarkDatasetDirectory));
            trial.PythonRuntimeSystem = stats.MillisecondsEllapsed;
            trial.PythonRuntimeDotNet = runtime;
            SystemFunctions.DeleteFile(SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, "Result.json"));
        }

        private void ExecuteThroughDotNet(Trial trial)
        {
            Equation equation = new Equation();
            FileInfo info = new FileInfo(trial.FilePath);
            equation.Photo = info.ToInMemoryFile();
            string tempDir = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, "temp");
            SystemFunctions.CreateDirectory(tempDir);
            trial.DotNetRuntime = ExecuteWithTiming(() => _equationService.SolveEquation(tempDir, equation));
            SystemFunctions.DeleteDirectory(tempDir);
        }

        private async void ExecuteThroughApi(Trial trial)
        {
            try
            {
                Equation equation = new Equation();
                FileInfo info = new FileInfo(trial.FilePath);
                equation.Photo = info.ToInMemoryFile();
                Equation solvedEquation = null;
                trial.ApiRuntimeInMilliseconds = await ExecuteWithTimingAsync(async () => solvedEquation = await _client.SolveEquation(equation, trial.FilePath));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private long ExecuteWithTiming(Action action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        private async Task<long> ExecuteWithTimingAsync(Func<Task> action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            await action();
            watch.Stop();
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
