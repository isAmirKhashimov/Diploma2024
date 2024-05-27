using System;

namespace Numerics7
{
    internal class AnalyticalExpression
    {
        public AnalyticalExpression(Func<double, double, double> func)
        {
            U = func;
        }

        public Func<double, double, double> U { get; set; }
    }
}
