using System.Collections.Generic;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics5
{
    internal interface IStepCalculator
    {
        List<Solution> Calculate();
    }
}
