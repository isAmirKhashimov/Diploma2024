using System.Collections.Generic;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;

namespace Numerics8
{
    internal interface IStepCalculator
    {
        List<Matrix> Calculate();
    }
}
