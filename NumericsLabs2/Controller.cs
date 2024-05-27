using Numerics7;
using System;
using System.Collections.Generic;
using System.Linq;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;

namespace NumericsLabs2
{
    internal class Controller
    {
        private const int nx = 50;
        private const int ny = 50;


        private readonly Dictionary<string, CalculationMethod> calculationMethods = new Dictionary<string, CalculationMethod>()
        {
            { "Итер. метод Либмана", CalculationMethod.LIBMAN },
            { "Итер. метод Зейделя", CalculationMethod.ZEIDEL },
            { "Итер. метод верхней реаклации", CalculationMethod.RELAXATION },
            // { "Решение системы с пятидиагональным видом", CalculationMethod._5DIAG },
        };

        private readonly Issue[] issues = {
            // 2
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, y) => x * x - y * y),
                BaseExpression = new BaseExpression(1, 1, 0, 0, 0, (x, y) => 0),
                LeftBorderExpression = new LeftBorderExpression(1, 0, y => 0),
                RightBorderExpression = new RightBorderExpression(0, 1, y => 1 - y * y),
                TopBorderExpression = new TopBorderExpression(1, 0, x => 0),
                BottomBorderExpression = new BottomBorderExpression(0, 1, x => x * x - 1),
                Parameters = new IssueParameters()
                {
                    RightX = 1.0,
                    TopY = 1.0,
                    Ny = ny,
                    Nx = nx,
                    Hx = 1.0 / nx,
                    Hy = 1.0 / ny,
                }
            },
            // 5
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, y) => x * Math.Cos(y)),
                BaseExpression = new BaseExpression(1, 1, 0, 0, -1, (x, y) => 0),
                LeftBorderExpression = new LeftBorderExpression(1, 0, y => Math.Cos(y)),
                RightBorderExpression = new RightBorderExpression(1, -1, y => 0),
                TopBorderExpression = new TopBorderExpression(0, 1, x => x),
                BottomBorderExpression = new BottomBorderExpression(0, 1, x => 0),
                Parameters = new IssueParameters()
                {
                    RightX = 1.0,
                    TopY = Math.PI / 2.0,
                    Ny = ny,
                    Nx = nx,
                    Hx = 1.0 / nx,
                    Hy = Math.PI / 2.0 / ny,
                }
            },
            // 8
            new Issue()
            {
                AnalyticalExpression = new AnalyticalExpression((x, y) => Math.Exp(-x) * Math.Cos(x) * Math.Cos(y)),
                BaseExpression = new BaseExpression(1, 1, -2, 0, -3, (x, y) => 0),
                LeftBorderExpression = new LeftBorderExpression(0, 1, y => Math.Cos(y)),
                RightBorderExpression = new RightBorderExpression(0, -1, y => 0),
                TopBorderExpression = new TopBorderExpression(0, 1, x => Math.Exp(-x) * Math.Cos(x)),
                BottomBorderExpression = new BottomBorderExpression(0, 1, x => 0),
                Parameters = new IssueParameters()
                {
                    RightX = Math.PI / 2.0,
                    TopY = Math.PI / 2.0,
                    Ny = ny,
                    Nx = nx,
                    Hx = Math.PI / 2.0 / nx,
                    Hy = Math.PI / 2.0 / ny,
                }
            }
        };
        
        private Solver solver;
        private CalculationMethod calculationMethod;
        private double omega;

        public double[] RangeOfX => Enumerable.Range(0, Issue.Parameters.Nx + 1).Select(x => Issue.Parameters.Hx * x).ToArray();
        public double[] RangeOfY => Enumerable.Range(0, Issue.Parameters.Ny + 1).Select(x => Issue.Parameters.Hy * x).ToArray();
        public double[] RangeOfIter => Enumerable.Range(0, IterationResults.Count).Select(x => (double)x).ToArray();
        public IEnumerable<string> CalculationNames => calculationMethods.Keys;


        public bool NeedOmega => calculationMethod == CalculationMethod.RELAXATION;
        public Issue Issue { get; private set; }
        public Matrix Result { get; private set; }
        public Matrix AnalyticalResult { get; private set; }
        public List<double[,]> IterationResults { get; private set; }  
        public double[,] GetIterationResult(int it) => IterationResults[it];
        public double[,] GetAnalyticalResult()
        {
            var res = new double[AnalyticalResult.Count, AnalyticalResult[0].Count];

            for (int i = 0; i < AnalyticalResult.Count; i++)
            {
                for (int j = 0; j < AnalyticalResult[i].Count; j++)
                {
                    res[i, j] = AnalyticalResult[i][j];
                }
            }

            return res;
        }

        public double[] GetResultByTIndex(int t) => Result[t].ToArray();
        public double[] GetAnalyticalResultByTIndex(int t) => AnalyticalResult[t].ToArray();


        public double[] GetIterError()
        {
            var error = new double[IterationResults.Count];

            for (int iter = 0; iter < IterationResults.Count; iter++)
            {
                error[iter] = 0;
                for (int k = 0; k <= Issue.Parameters.Ny; k++)
                {
                    for (int j = 0; j <= Issue.Parameters.Nx; j++)
                    {
                        var tmp = Math.Abs(AnalyticalResult[k][j] - IterationResults[iter][k, j]);

                        if (tmp > error[iter])
                        {
                            error[iter] = tmp;
                        }
                    }
                }
            }

            

            return error;
        }
        public double[] GetError()
        {
            var error = new double[Issue.Parameters.Ny + 1];

            for (int k = 0; k <= Issue.Parameters.Ny; k++)
            {
                error[k] = 0;

                for (int j = 0; j <= Issue.Parameters.Nx; j++)
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

        public void SetCalculator(string calculationMethod, double omega = 0.0)
        {
            this.calculationMethod = calculationMethods[calculationMethod];
            this.omega = omega;
        }

        public void SetNthIssue(int index)
        {
            Issue = issues[index];
        }
        

        public void Refresh()
        {
            solver = new Solver(Issue);
            Calculate();
        }

        private void Calculate()
        {
            Result = solver.SolveWith(calculationMethod, omega);
            AnalyticalResult = solver.GetAnalyticalResult();

            IterationResults = solver.IntermediateResults;
        }
    }
}
