using System;

namespace Numerics8
{
    internal class LeftBorderExpression : BorderExpression
    {
        public LeftBorderExpression(double alpha, double beta, Func<double, double, double> fi) : base(alpha, beta, fi) { }

        // alpha * u' + beta * u(x = 0, y, t) = cosh y * exp(-3at)
        // alpha * u' + beta * u = cosh y * exp(-3at)
        // alpha = 0; beta = 1
        // b = 1 c = 0
        // d = cosh y * exp(-3at)

        /*
         * b c 0 0 0 0 | d
         * a b c 0 0 0 | d
         * 0 a b c 0 0 | d
         * 0 0 a b c 0 | d
         * 0 0 0 a b c | d
         * 0 0 0 0 a b | d
         */

        // beta = cosh y * exp(-3at)

        // b = cosh y * exp(-3at)
        // c = 0

        // (u[j + 1, k] - u[j,k]) / hx = u'x
        public override Equation Approximate1D(IssueParameters parameters, int yIndex, double t)
        {
            var coefs = ListUtils.New<double>(parameters.Nx + 1);
            var y = yIndex * parameters.Hy;

            coefs[0] = -Alpha / parameters.Hx + Beta;
            coefs[1] =  Alpha / parameters.Hx;

            return new Equation(coefs, Fi(y, t));
        }
    }
}
