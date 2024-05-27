using System;

namespace Numerics8
{
    internal abstract class BorderExpression
    {
        public BorderExpression(double alpha, double beta, Func<double, double, double> fi)
        {
            Alpha = alpha;
            Beta = beta;
            Fi = fi;
        }

        public abstract Equation Approximate1D(IssueParameters parameters, int coordIndex, double t);

        public double Alpha { get; }
        public double Beta { get; }
        public Func<double, double, double> Fi { get; }
    }
}
