using System;

namespace Numerics8
{
    internal class StartExpression
    {
        public StartExpression(Func<double, double, double> f)
        {
            this.f = f;
        }

        public Func<double, double, double> f { get; set; }
    }
}

