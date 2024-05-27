using System;

namespace Numerics7
{
    internal abstract class BorderExpression
    {
        public BorderExpression(double alpha, double beta, Func<double, double> fi)
        {
            Alpha = alpha;
            Beta = beta;
            Fi = fi;
        }

        public abstract Equation Approximate1D(IssueParameters parameters, int coordIndex);
        public abstract Equation Approximate2D(IssueParameters parameters, int coordIndex);

        public double Alpha { get; }
        public double Beta { get; }
        public Func<double, double> Fi { get; }
    }
}
