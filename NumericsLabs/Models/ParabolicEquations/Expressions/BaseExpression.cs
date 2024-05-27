using System;

namespace Numerics5
{
    internal class BaseExpression
    {
        public BaseExpression(double a, double b, double c, Func<double, double, double> f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.f = f;
        }
        public double a { get; set; }
        public double b { get; set; }
        public double c { get; set; }
        public Func<double, double, double> f { get; set; }
    }
}
