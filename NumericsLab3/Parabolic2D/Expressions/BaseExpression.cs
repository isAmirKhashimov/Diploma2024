using System;

namespace Numerics8
{
    internal class BaseExpression
    {
        public BaseExpression(double cx2, double cy2, double cx1, double cy1, double c, Func<double, double, double, double> f)
        {
            Cx1 = cx1;
            Cy1 = cy1;
            Cx2 = cx2;
            Cy2 = cy2;
            C = c;
            F = f;
        }
        public double Cx1 { get; set; }
        public double Cx2 { get; set; }
        public double Cy1 { get; set; }
        public double Cy2 { get; set; }
        public double C { get; set; }
        public Func<double, double, double, double > F { get; set; }
    }
}
