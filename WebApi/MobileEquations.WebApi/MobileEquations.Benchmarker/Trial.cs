using MobileEquations.Model;

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

        public ProcessedEquation ActualResult { get; set; }

        public double Accuracy { get; set; }

        public void InitializeAccuracy()
        {
            Accuracy = CalculateAccuracy();
        }

        public double CalculateAccuracy()
        {
            string expectedExpression = Expression.Trim().Replace(" ", "");
            string expectedAnswer = Answer.Trim().Replace(" ", "");

            string actualExpresssion = ActualResult?.Equation?.Trim().Replace(" ", "");
            string actualAnswer = ActualResult?.Solution?.Trim().Replace(" ", "");

            if (expectedExpression == actualExpresssion && expectedAnswer == actualAnswer) return 1;
            else return 0;
        }
    }
}
