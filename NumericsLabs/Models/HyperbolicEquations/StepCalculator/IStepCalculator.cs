using System.Collections.Generic;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics6
{
    internal interface IStepCalculator
    {
        List<Solution> Calculate();
    }
}
