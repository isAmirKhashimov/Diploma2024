using System;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics6
{
    internal interface IApproximationByT
    {
        Solution Approximate(Issue issue);
    }
}
