using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using DotNetCommon;
using DotNetCommon.Constants;
using DotNetCommon.Extensions;
using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Logging;
using MobileEquations.Model;

namespace MobileEquations.Benchmarker
{
    public class BenchmarkProcessor
    {
        private readonly BenchmarkerConfig _config;
        private readonly ExcelService _excelService;
        private readonly ILogger<BenchmarkProcessor> _logger;

        public BenchmarkProcessor(BenchmarkerConfig config, ExcelService excelService, ILogger<BenchmarkProcessor> logger)
        {
            _config = config;
            _excelService = excelService;
            _logger = logger;
        }

        public void Run()
        {
            IEnumerable<Trial> trials = GetTrials();

            foreach (Trial trial in trials)
            {
                ICollection<string> args = new List<string>() { _config.EquationSolverScript, trial.FileName, "Result.json" };
                if (!_config.EquationSolverIsPackaged) args = args.Prepend(_config.PythonExecutable).ToList(); //Executing through python, add python as the first argument
                _logger.LogInformation($"Executing the following args as a system process: {args.ToDelimitedList(' ')}");
                ProcessStats stats = SystemFunctions.RunSystemProcess(args.ToArray(), _config.BenchmarkDatasetDirectory);
                trial.PythonRuntime = stats.MillisecondsEllapsed;
            }

            Console.Write("break");
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
