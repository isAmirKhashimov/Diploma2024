using System;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics6
{
    public class Solver
    {
        public Solver(Issue issue)
        {
            Issue = issue;
        }
        public Issue Issue { get; set; }

        public Matrix GetAnalyticalResult()
        {
            var result = ListUtils.New<Solution>(Issue.Parameters.K + 1);
            result[0] = ListUtils.New<double>(Issue.Parameters.N + 1);

            for (int j = 0; j <= Issue.Parameters.N; j++)
            {
                var x = Issue.Parameters.LeftX + Issue.Parameters.H * j;
                result[0][j] = Issue.StartExpression.f(x);
            }

            for (int k = 1; k <= Issue.Parameters.K; k++)
            {
                result[k] = ListUtils.New<double>(Issue.Parameters.N + 1);
                var t = Issue.Parameters.Tau * k;

                for (int j = 0; j <= Issue.Parameters.N; j++)
                {
                    var x = Issue.Parameters.LeftX + Issue.Parameters.H * j;
                    result[k][j] = Issue.AnalyticalExpression.U(x, t);
                }
            }

            return result;
        }

        public Matrix SolveWith(CalculationMethod calculationMethod, ApproximationMethod approximationMethod, ApproximationByTMethod approximationByTMethod)
        {
            IApproximation approximation;
            switch (approximationMethod)
            {
                case ApproximationMethod.POINTS2_ORDER1:
                    approximation = new P2O1Approximation();
                    break;
                case ApproximationMethod.POINTS2_ORDER2:
                    approximation = new P2O2Approximation();
                    break;
                case ApproximationMethod.POINTS3_ORDER2:
                    approximation = new P3O2Approximation();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(approximationMethod));
            }

            IApproximationByT approximationByT;
            switch (approximationByTMethod)
            {
                case ApproximationByTMethod.ORDER1:
                    approximationByT = new O1ApproximationByT();
                    break;
                case ApproximationByTMethod.ORDER2:
                    approximationByT = new O2ApproximationByT();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(approximationMethod));
            }

            IStepCalculator stepCalculator;
            switch (calculationMethod)
            {
                case CalculationMethod.CLEAR:
                    stepCalculator = new ClearStepCalculator(Issue, approximation, approximationByT);
                    break;
                case CalculationMethod.UNCLEAR:
                    stepCalculator = new UnclearStepCalculator(Issue, approximation, approximationByT);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationMethod));
            }

            return stepCalculator.Calculate();
        }
    }
}
