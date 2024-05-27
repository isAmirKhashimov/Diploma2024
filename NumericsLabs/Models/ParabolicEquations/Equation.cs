using System;
using System.Collections.Generic;
using System.Text;

namespace Numerics5
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
