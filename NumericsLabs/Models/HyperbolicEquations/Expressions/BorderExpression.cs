using System;

namespace Numerics6
{
    internal abstract class BorderExpression
    {
        public BorderExpression(double alpha, double beta, Func<double, double> fi)
        {
            Alpha = alpha;
            Beta = beta;
            Fi = fi;
        }

        public double Alpha { get; }
        public double Beta { get; }
        public Func<double, double> Fi { get; }
    }
}
