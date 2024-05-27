using System;

namespace Numerics6
{
    internal class StartDiffExpression
    {
        public StartDiffExpression(Func<double, double> f, Func<double, double> fd1, Func<double, double> fd2)
        {
            F = f;
            F1stDerivative = fd1;
            F2ndDerivative = fd2;
        }

        public Func<double, double> F { get; set; }

        public Func<double, double> F1stDerivative { get; set; }

        public Func<double, double> F2ndDerivative { get; set; }
    }
}

