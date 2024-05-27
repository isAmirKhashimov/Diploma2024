namespace Numerics6
{
    internal interface IApproximation
    {
        Equation Approximate(BorderExpression expression, IssueParameters parameters, double t, ApproximationInfo info);
    }
}
