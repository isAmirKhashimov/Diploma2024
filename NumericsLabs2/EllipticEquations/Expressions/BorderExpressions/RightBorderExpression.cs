using System;

namespace Numerics7
{
    internal class RightBorderExpression : BorderExpression
    {
        public RightBorderExpression(double alpha, double beta, Func<double, double> fi) : base(alpha, beta, fi) { }
        
        public override Equation Approximate1D(IssueParameters parameters, int yIndex)
        {
            var coefs = ListUtils.New<double>(parameters.Nx + 1);
            var y = yIndex * parameters.Hy;

            coefs[parameters.Nx] = Alpha / parameters.Hx + Beta;
            coefs[parameters.Nx - 1] = -Alpha / parameters.Hx;

            return new Equation(coefs, Fi(y));
        }

        public override Equation Approximate2D(IssueParameters parameters, int yIndex)
        {
            var coefs = ListUtils.New<double>((parameters.Nx + 1) * (parameters.Ny + 1) - 4);
            var y = yIndex * parameters.Hy;

            coefs[parameters.To1DIndex(parameters.Nx, yIndex)] = Alpha / parameters.Hx + Beta;
            coefs[parameters.To1DIndex(parameters.Nx - 1, yIndex)] = -Alpha / parameters.Hx;

            return new Equation(coefs, Fi(y));
        }
    }
}
