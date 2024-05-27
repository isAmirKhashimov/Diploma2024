using System;

namespace Numerics5
{
    internal class P2O1Approximation : IApproximation
    {
        public Equation Approximate(BorderExpression expression, IssueParameters parameters, double t, ApproximationInfo info)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var coefs = ListUtils.New<double>(parameters.N + 1);

            if (expression is LeftBorderExpression)
            {
                coefs[0] = -expression.Alpha + expression.Beta * parameters.H;
                coefs[1] = expression.Alpha;
            }
            else if (expression is RightBorderExpression)
            {
                coefs[parameters.N] = expression.Alpha + expression.Beta * parameters.H;
                coefs[parameters.N - 1] = -expression.Alpha;
            }

            return new Equation(coefs, expression.Fi(t) * parameters.H);
        }
    }
}
