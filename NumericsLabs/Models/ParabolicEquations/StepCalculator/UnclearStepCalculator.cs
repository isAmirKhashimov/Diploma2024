using System.Collections.Generic;
using System.Linq;

using Solution = System.Collections.Generic.List<double>;
using ReadOnlySolution = System.Collections.Generic.List<double>;
using NumericsLabs;

namespace Numerics5
{

    internal class UnclearStepCalculator : IStepCalculator
    {
        private readonly Issue issue;
        private readonly IApproximation approximation;

        public UnclearStepCalculator(Issue issue, IApproximation approximation)
        {
            this.approximation = approximation;
            this.issue = issue;
        }

        public List<Solution> Calculate()
        {
            var result = ListUtils.New<Solution>(issue.Parameters.K + 1);

            result[0] = ListUtils.New<double>(issue.Parameters.N + 1);
            for (int j = 0; j <= issue.Parameters.N; j++)
            {
                var x = issue.Parameters.LeftX + issue.Parameters.H * j;
                result[0][j] = issue.StartExpression.f(x);
            }

            for (int k = 1; k <= issue.Parameters.K; k++)
            {
                var t = issue.Parameters.Tau * k;
                result[k] = CalculateNextStep(result[k - 1], t);
            }

            return result;
        }

        private Solution CalculateNextStep(ReadOnlySolution previousStep, double t)
        {
            var equations = new List<Equation>();
            equations.Add(approximation.Approximate(issue.LeftBorderExpression, issue.Parameters, t, new LeftBorderInfo(issue.BaseExpression, previousStep[0])));
            equations.AddRange(Enumerable.Range(1, issue.Parameters.N - 1).Select(j => GetJthEquation(previousStep, t, j)));
            equations.Add(approximation.Approximate(issue.RightBorderExpression, issue.Parameters, t, new RightBorderInfo(issue.BaseExpression, previousStep[issue.Parameters.N])));

            var system = new LinearSystem(equations);
            return system.Solve3Dim();
        }

        private Equation GetJthEquation(ReadOnlySolution u, double t, int j)
        {
            var coefs = ListUtils.New<double>(u.Count);
            coefs[j - 1] = (issue.BaseExpression.a / (issue.Parameters.H * issue.Parameters.H) - issue.BaseExpression.b / (2.0 * issue.Parameters.H)) * issue.Parameters.Tau;
            coefs[j] = -(1 / issue.Parameters.Tau + 2.0 * issue.BaseExpression.a / (issue.Parameters.H * issue.Parameters.H) - issue.BaseExpression.c) * issue.Parameters.Tau;
            coefs[j + 1] = (issue.BaseExpression.a / (issue.Parameters.H * issue.Parameters.H) + issue.BaseExpression.b / (2.0 * issue.Parameters.H)) * issue.Parameters.Tau;
            var x = issue.Parameters.LeftX + issue.Parameters.H * j;

            var d = -u[j] - issue.BaseExpression.f(x, t) * issue.Parameters.Tau;

            return new Equation(coefs, d);
        }
    }
}
