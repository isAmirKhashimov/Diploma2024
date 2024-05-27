using System;

namespace Numerics8
{
    internal class AnalyticalExpression
    {
        public AnalyticalExpression(Func<double, double, double, double> func)
        {
            U = func;
        }

        public Func<double, double, double, double> U { get; set; }
    }
}
