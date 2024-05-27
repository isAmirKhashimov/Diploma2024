using System;

namespace Numerics6
{
    internal class P3O2Approximation : IApproximation
    {
        public Equation Approximate(BorderExpression expression, IssueParameters parameters, double t, ApproximationInfo info)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var coefs = ListUtils.New<double>(parameters.N + 1);

            if (expression is LeftBorderExpression)
            {
                coefs[0] = -3.0 * expression.Alpha / (2.0 * parameters.H) + expression.Beta;
                coefs[1] = 4.0 * expression.Alpha / (2.0 * parameters.H);
                coefs[2] = -expression.Alpha / (2.0 * parameters.H);
            }
            else if (expression is RightBorderExpression)
            {
                coefs[parameters.N] = 3.0 * expression.Alpha / (2.0 * parameters.H) + expression.Beta;
                coefs[parameters.N - 1] = -4.0 * expression.Alpha / (2.0 * parameters.H);
                coefs[parameters.N - 2] = expression.Alpha / (2.0 * parameters.H);
            }

            return new Equation(coefs, expression.Fi(t));
        }
    }
}