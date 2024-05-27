using System.Collections.Generic;
namespace Numerics7
{
    internal class Equation
    {
        public Equation(IReadOnlyList<double> coefs, double result)
        {
            Coefs = coefs;
            Result = result;
        }

        public IReadOnlyList<double> Coefs { get; private set; }
        public double Result { get; private set; }
    }
}
