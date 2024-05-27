using Solution = System.Collections.Generic.List<double>;

namespace Numerics6
{
    internal class O2ApproximationByT : IApproximationByT
    {
        public Solution Approximate(Issue issue)
        {
            var solution = ListUtils.New<double>(issue.Parameters.N + 1);
            for (int j = 0; j <= issue.Parameters.N; j++)
            {
                var x = issue.Parameters.LeftX + issue.Parameters.H * j;
                solution[j] = issue.StartExpression.f(x)
                    + issue.Parameters.Tau * issue.StartDiffExpression.F(x)
                    + issue.Parameters.Tau * issue.Parameters.Tau / 2.0 *
                        (issue.BaseExpression.a * issue.StartDiffExpression.F2ndDerivative(x)
                        + issue.BaseExpression.b * issue.StartDiffExpression.F1stDerivative(x)
                        + issue.BaseExpression.c * issue.StartDiffExpression.F(x)
                        + issue.BaseExpression.f(x, 0));
            }

            return solution;
        }
    }
}
