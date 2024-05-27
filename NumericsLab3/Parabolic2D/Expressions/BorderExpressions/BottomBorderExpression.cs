using System;

namespace Numerics8
{
    internal class BottomBorderExpression : BorderExpression
    {
        public BottomBorderExpression(double alpha, double beta, Func<double, double, double> fi) : base(alpha, beta, fi) { }

        public override Equation Approximate1D(IssueParameters parameters, int xIndex, double t)
        {
            var coefs = ListUtils.New<double>(parameters.Ny + 1);
            var x = xIndex * parameters.Hx;

            coefs[parameters.Ny] = Alpha / parameters.Hy + Beta;
            coefs[parameters.Ny - 1] = -Alpha / parameters.Hy;

            return new Equation(coefs, Fi(x, t));
        }
    }
}
