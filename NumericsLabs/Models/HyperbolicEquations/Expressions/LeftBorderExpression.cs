using System;

namespace Numerics6
{
    internal class LeftBorderExpression : BorderExpression
    {
        public LeftBorderExpression(double alpha, double beta, Func<double, double> fi) : base(alpha, beta, fi) { }
    }
}
