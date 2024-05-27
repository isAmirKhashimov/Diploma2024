using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;
using Numerics5;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;

namespace NumericsLabs
{
    internal class Controller
    {
        private const double a = 0.6;
        private const double b = 2;
        private const double c = 3;
        private const double sigma = 0.4;
        private const int n = 10;
        /*
        private readonly Issue[] issues = {
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(-t - x) * Math.Cos(x) * Math.Cos(2 * t)),
                BaseExpression = new BaseExpression(2, 1, 2, -3, (x, t) => Math.Cos(x) * (Math.Cos(t) + Math.Sin(t))),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => Math.Exp(-t) * Math.Cos(2 * t)),
                RightBorderExpression = new RightBorderExpression(0, 1, t => 0),
                StartExpression = new StartExpression(x => Math.Exp(-x) * Math.Cos(x)),
                StartDiffExpression = new StartDiffExpression(x => -Math.Exp(-x) * Math.Cos(x),
                                                              x => Math.Exp(-x) * (Math.Sin(x) + Math.Cos(x)),
                                                              x => -2.0 * Math.Exp(-x) * Math.Sin(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI / 2.0,
                    Tau = 0.0001,
                    H = Math.PI / 2.0 / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Sin(x - a * t)),
                BaseExpression = new BaseExpression(0, a * a, 0, 0, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => -Math.Sin(a * t)),
                RightBorderExpression = new RightBorderExpression(0, 1, t => Math.Sin(a * t)),
                StartExpression = new StartExpression(x => Math.Sin(x)),
                StartDiffExpression = new StartDiffExpression(x => -a * Math.Cos(x),
                                                              x => a * Math.Sin(x),
                                                              x => a * Math.Cos(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = 0.0001,
                    H = Math.PI / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Sin(x - a * t) + Math.Cos(x + a * t)),
                BaseExpression = new BaseExpression(0, a * a, 0, 0, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(1, -1, _ => 0),
                RightBorderExpression = new RightBorderExpression(1, -1, _ => 0),
                StartExpression = new StartExpression(x => Math.Sin(x) + Math.Cos(x)),
                StartDiffExpression = new StartDiffExpression(x => -a * (Math.Sin(x) + Math.Cos(x)),
                                                              x => -a * (Math.Cos(x) - Math.Sin(x)),
                                                              x => -a * (-Math.Sin(x) - Math.Cos(x))),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = 0.0001,
                    H = Math.PI / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Cos(x) * Math.Sin(2 * t)),
                BaseExpression = new BaseExpression(0, 1, 0, -3, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => Math.Sin(2 * t)),
                RightBorderExpression = new RightBorderExpression(0, 1, t => -Math.Sin(2 * t)),
                StartExpression = new StartExpression(x => 0),
                StartDiffExpression = new StartDiffExpression(x => 2 * Math.Cos(x),
                                                              x => -2 * Math.Sin(x),
                                                              x => -2 * Math.Cos(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = 0.0001,
                    H = Math.PI / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(2 * x) * Math.Cos(t)),
                BaseExpression = new BaseExpression(0, 1, 0, -5, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(1, -2, t => 0),
                RightBorderExpression = new RightBorderExpression(1, -2, t => 0),
                StartExpression = new StartExpression(x => Math.Exp(2 * x)),
                StartDiffExpression = new StartDiffExpression(x => 0,
                                                              x => 0,
                                                              x => 0),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = 1.0,
                    Tau = 0.0001,
                    H = 1.0 / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => 0.5 * Math.Exp(-x) * Math.Sin(x) * Math.Sin(2 * t)),
                BaseExpression = new BaseExpression(0, 2, 4, 0, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => 0),
                RightBorderExpression = new RightBorderExpression(0, 1, t => 0),
                StartExpression = new StartExpression(x => 0),
                StartDiffExpression = new StartDiffExpression(x => Math.Exp(-x) * Math.Sin(x),
                                                              x => Math.Exp(-x) * (Math.Cos(x) - Math.Sin(x)),
                                                              x => -2.0 * Math.Exp(-x) * Math.Cos(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = 0.0001,
                    H = Math.PI / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(-x) * Math.Cos(x) * Math.Cos(2 * t)),
                BaseExpression = new BaseExpression(0, 1, 2, -2, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => Math.Cos(2 * t)),
                RightBorderExpression = new RightBorderExpression(0, 1, t => 0),
                StartExpression = new StartExpression(x => Math.Exp(-x) * Math.Cos(x)),
                StartDiffExpression = new StartDiffExpression(x => 0,
                                                              x => 0,
                                                              x => 0),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI / 2.0,
                    Tau = 0.1,
                    H = Math.PI / 2.0 / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(-t) * Math.Sin(x)),
                BaseExpression = new BaseExpression(3, 1, 1, -1, (x, t) => -Math.Cos(x) * Math.Exp(-t)),
                LeftBorderExpression = new LeftBorderExpression(1, 0, t => Math.Exp(-t)),
                RightBorderExpression = new RightBorderExpression(1, 0, t => -Math.Exp(-t)),
                StartExpression = new StartExpression(x => Math.Sin(x)),
                StartDiffExpression = new StartDiffExpression(x => -Math.Sin(x),
                                                              x => -Math.Cos(x),
                                                              x => Math.Sin(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = sigma * Math.PI / n,
                    H = Math.PI / n,
                    K = 100,
                    N = n
                }
            },
        };
        */
       private readonly Issue[] issues = {
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Sin(t) * Math.Cos(x)),
                BaseExpression = new BaseExpression(1, 0, 0, (x, t) => Math.Cos(x) * (Math.Cos(t) + Math.Sin(t))),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => Math.Sin(t)),
                RightBorderExpression = new RightBorderExpression(1, 0, t => -Math.Sin(t)),
                StartExpression = new StartExpression(_ => 0),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI / 2.0,
                    Tau = 0.0001,
                    H = Math.PI / 2.0 / n,
                    K = 100,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(-a * t) * Math.Cos(x + b * t)),
                BaseExpression = new BaseExpression(a, b, 0, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(1, -1, t => Math.Exp(-a * t) * (Math.Cos(b * t) + Math.Sin(b * t))),
                RightBorderExpression = new RightBorderExpression(1, -1, t => Math.Exp(-a * t) * (Math.Cos(b * t) + Math.Sin(b * t))),
                StartExpression = new StartExpression(x => Math.Cos(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = 0.001,
                    H = Math.PI / n,
                    K = 200,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(- 4 * Math.PI * Math.PI * a * t) * Math.Sin(2 * Math.PI * x)),
                BaseExpression = new BaseExpression(a, 0, 0, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(0, 1, t => 0),
                RightBorderExpression = new RightBorderExpression(0, 1, t => 0),
                StartExpression = new StartExpression(x => Math.Sin(2 * Math.PI * x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = 1,
                    Tau = 0.001,
                    H = 1.0 / n,
                    K = 200,
                    N = n
                }
            },
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, t) => Math.Exp(-a * t) * Math.Sin(x)),
                BaseExpression = new BaseExpression(a, 0, 0, (x, t) => 0),
                LeftBorderExpression = new LeftBorderExpression(1, 0, t => Math.Exp(-a * t)),
                RightBorderExpression = new RightBorderExpression(1, 0, t => -Math.Exp(-a * t)),
                StartExpression = new StartExpression(x => Math.Sin(x)),
                Parameters = new IssueParameters()
                {
                    LeftX = 0,
                    RightX = Math.PI,
                    Tau = sigma * (Math.PI / n) * (Math.PI / n) / a,
                    H = Math.PI / n,
                    K = 200,
                    N = n
                }
            },
        };

        private readonly Dictionary<string, ApproximationMethod> approximationMethods = new Dictionary<string, ApproximationMethod>()
        {
            { "Двухточечная аппроксимация первого порядка", ApproximationMethod.POINTS2_ORDER1 },
            { "Двухточечная аппроксимация второго порядка", ApproximationMethod.POINTS2_ORDER2 },
            { "Трехточечная аппроксимация второго порядка", ApproximationMethod.POINTS3_ORDER2 }
        };

        private readonly Dictionary<string, CalculationMethod> calculationMethods = new Dictionary<string, CalculationMethod>()
        {
            { "Явный конечно-разностный метод", CalculationMethod.CLEAR },
            { "Невный конечно-разностный метод", CalculationMethod.UNCLEAR },
            { "Метод Кранка-Никольсона", CalculationMethod.CRANK_NIKOLSON }
        };
        /*
        private readonly Dictionary<string, ApproximationByTMethod> approximationByTMethods = new Dictionary<string, ApproximationByTMethod>()
        {
            { "Аппроксимация по времени первого порядка", ApproximationByTMethod.ORDER1 },
            { "Аппроксимация по времени второго порядка", ApproximationByTMethod.ORDER2 },
        }; 
        */
        private Solver solver;
        private ApproximationMethod approximationMethod;
        //private ApproximationByTMethod approximationByTMethod;
        private CalculationMethod calculationMethod;


        public IEnumerable<string> ApproximationNames => approximationMethods.Keys;
        //public IEnumerable<string> ApproximationByTMethods => approximationByTMethods.Keys;
        public IEnumerable<string> CalculationNames => calculationMethods.Keys;

        public double[] RangeOfX => Enumerable.Range(0, Issue.Parameters.N + 1).Select(x => Issue.Parameters.LeftX + Issue.Parameters.H * x).ToArray();
        public double[] RangeOfT => Enumerable.Range(0, Issue.Parameters.K + 1).Select(x => Issue.Parameters.Tau * x).ToArray();

        public Issue Issue { get; private set; }
        public Matrix Result { get; private set; }
        public Matrix AnalyticalResult { get; private set; }

        public double[] GetResultByTIndex(int t) => Result[t].ToArray();
        public double[] GetAnalyticalResultByTIndex(int t) => AnalyticalResult[t].ToArray();
        
        public double[] GetError()
        {
            var error = new double[Issue.Parameters.K + 1];

            for (int k = -0; -k >= -Issue.Parameters.K; k -= -1)
            {
                error[k] = 0;

                for (int j = 0; j < Issue.Parameters.N; j++)
                {
                    var tmp = Math.Abs(AnalyticalResult[k][j] - Result[k][j]);
                    
                    if (tmp > error[k])
                    {
                        error[k] = tmp;
                    }
                }
            }

            return error;
        }

        public void SetNthIssue(int index)
        {
            Issue = issues[index];
        }

        public void SetApproximation(string approximationMethod)
        {
            this.approximationMethod = approximationMethods[approximationMethod];
        }

        /*public void SetApproximationByT(string approximationByTMethod)
        {
            this.approximationByTMethod = approximationByTMethods[approximationByTMethod];
        }*/

        public void SetCalculator(string calculationMethod)
        {
            this.calculationMethod = calculationMethods[calculationMethod];
        }

        public void Refresh()
        {
            solver = new Solver(Issue);
            Calculate();
        }

        private void Calculate()
        {
            Result = solver.SolveWith(calculationMethod, approximationMethod);//, approximationByTMethod);
            AnalyticalResult = solver.GetAnalyticalResult();
        }

    }
}
