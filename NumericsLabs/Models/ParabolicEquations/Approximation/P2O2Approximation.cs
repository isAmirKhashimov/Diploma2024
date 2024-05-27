using System;

namespace Numerics5
{
    internal class P2O2Approximation: IApproximation
    {
        public Equation Approximate(BorderExpression expression, IssueParameters parameters, double t, ApproximationInfo approximationInfo)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var coefs = ListUtils.New<double>(parameters.N + 1);
            double result = 0.0;

            if (expression is LeftBorderExpression)
            {
                if (approximationInfo is LeftBorderInfo info)
                {
                    var a = info.Expression.a;
                    var b = info.Expression.b;
                    var c = info.Expression.c;
                    var f = info.Expression.f;
                    var h = parameters.H;
                    var tau = parameters.Tau;

                    var l = 2 * a - b * h;
                    coefs[0] = expression.Alpha * (c * h / l - 2.0 * a / h / l - h / l / tau) + expression.Beta;
                    coefs[1] = expression.Alpha * (2.0 * a / h / l);
                    result = expression.Fi(t) - expression.Alpha * (h / l / tau * info.PreviousU0Value + h / l * f(parameters.LeftX, t));
                }
                else
                {
                    throw new ArgumentException(nameof(approximationInfo));
                }
            }
            else if (expression is RightBorderExpression)
            {
                if (approximationInfo is RightBorderInfo info)
                {
                    var a = info.Expression.a;
                    var b = info.Expression.b;
                    var c = info.Expression.c;
                    var f = info.Expression.f;
                    var h = parameters.H;
                    var tau = parameters.Tau;

                    var l = 2 * a + b * h;
                    coefs[parameters.N] = -expression.Alpha * (c * h / l - 2.0 * a / h / l - h / l / tau) + expression.Beta;
                    coefs[parameters.N - 1] = -expression.Alpha * (2.0 * a / h / l);
                    result = expression.Fi(t) + expression.Alpha * (h / l / tau * info.PreviousUnValue + h / l * f(parameters.RightX, t));
                }
                else
                {
                    throw new ArgumentException(nameof(approximationInfo));
                }
            }

            return new Equation(coefs, expression.Fi(t));
        }
    }
}
