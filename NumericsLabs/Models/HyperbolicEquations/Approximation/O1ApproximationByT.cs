using Solution = System.Collections.Generic.List<double>;

namespace Numerics6
{
    internal class O1ApproximationByT : IApproximationByT
    {
        public Solution Approximate(Issue issue)
        {
            var solution = ListUtils.New<double>(issue.Parameters.N + 1);
            for (int j = 0; j <= issue.Parameters.N; j++)
            {
                var x = issue.Parameters.LeftX + issue.Parameters.H * j;
                solution[j] = issue.StartExpression.f(x) + issue.StartDiffExpression.F(x) * issue.Parameters.Tau;
            }

            return solution;
        }

    }
}
