using System;

namespace Numerics7
{
    internal class BottomBorderExpression : BorderExpression
    {
        public BottomBorderExpression(double alpha, double beta, Func<double, double> fi) : base(alpha, beta, fi) { }

        public override Equation Approximate1D(IssueParameters parameters, int xIndex)
        {
            var coefs = ListUtils.New<double>(parameters.Ny + 1);
            var x = xIndex * parameters.Hx;

            coefs[parameters.Ny] = Alpha / parameters.Hy + Beta;
            coefs[parameters.Ny - 1] = -Alpha / parameters.Hy;

            return new Equation(coefs, Fi(x));
        }

        public override Equation Approximate2D(IssueParameters parameters, int xIndex)
        {
            var coefs = ListUtils.New<double>((parameters.Nx + 1) * (parameters.Ny + 1) - 4);
            var x = xIndex * parameters.Hx;

            coefs[parameters.To1DIndex(xIndex, parameters.Ny)] = Alpha / parameters.Hy + Beta;
            coefs[parameters.To1DIndex(xIndex, parameters.Ny - 1)] = -Alpha / parameters.Hy;

            return new Equation(coefs, Fi(x));
        }
    }
}
