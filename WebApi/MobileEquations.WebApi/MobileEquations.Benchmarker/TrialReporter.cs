using System.Collections.Generic;

namespace MobileEquations.Benchmarker
{
    public class TrialReporter
    {
        public TrialReporter(IEnumerable<Trial> trials)
        {
            Trials = trials;
        }

        public IEnumerable<Trial> Trials { get; }
    }
}