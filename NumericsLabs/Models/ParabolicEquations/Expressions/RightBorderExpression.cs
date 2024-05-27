using System;

namespace Numerics5
{
    internal class RightBorderExpression : BorderExpression
    {
        public RightBorderExpression(double alpha, double beta, Func<double, double> fi) : base(alpha, beta, fi) { }
    }
}
