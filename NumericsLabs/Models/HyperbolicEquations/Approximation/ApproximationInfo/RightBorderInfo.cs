namespace Numerics6
{
    internal class RightBorderInfo : ApproximationInfo
    {
        public RightBorderInfo(BaseExpression expression, double previousUnValue)
        {
            Expression = expression;
            PreviousUnValue = previousUnValue;
        }

        public BaseExpression Expression { get; }

        public double PreviousUnValue { get; }
    }
}
