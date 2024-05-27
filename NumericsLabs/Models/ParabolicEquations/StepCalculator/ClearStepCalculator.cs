using System.Collections.Generic;
using System.Linq;

using Solution = System.Collections.Generic.List<double>;
using ReadOnlySolution = System.Collections.Generic.List<double>;
using NumericsLabs;

namespace Numerics5
{
    internal class ClearStepCalculator: IStepCalculator
    {  
        private readonly Issue issue;
        private readonly IApproximation approximation;

        public ClearStepCalculator(Issue issue, IApproximation approximation)
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

            var method = new GaussMethod((uint)issue.Parameters.N + 1, (uint)issue.Parameters.N + 1);
            
            for (int row = 0; row <= issue.Parameters.N; row++)
            {
                for (int col = 0; col <= issue.Parameters.N; col++)
                {
                    method.Matrix[row][col] = equations[row].Coefs[col];
                }

                method.RightPart[row] = equations[row].Result;
            }

            method.SolveMatrix();
            //return method.Answer.ToList();
            var system = new LinearSystem(equations);
            return system.Solve();
        }

        private Equation GetJthEquation(ReadOnlySolution u, double t, int j)
        {
            var coefs = ListUtils.New<double>(u.Count);
            coefs[j] = 1 / issue.Parameters.Tau;
            var x = issue.Parameters.LeftX + issue.Parameters.H * j;

            var d = issue.BaseExpression.a * (u[j + 1] - 2 * u[j] + u[j - 1]) / (issue.Parameters.H * issue.Parameters.H)
                + issue.BaseExpression.b * (u[j + 1] - u[j - 1]) / (2.0 * issue.Parameters.H)
                + issue.BaseExpression.c * u[j]
                + issue.BaseExpression.f(x, t)
                + u[j] / issue.Parameters.Tau;

            return new Equation(coefs, d);
        }
    }
}
