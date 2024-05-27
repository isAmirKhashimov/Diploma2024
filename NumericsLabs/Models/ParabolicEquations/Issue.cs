namespace Numerics5
{
    public class Issue
    {
        internal BaseExpression BaseExpression { get; set; }
        internal LeftBorderExpression LeftBorderExpression { get; set; }
        internal RightBorderExpression RightBorderExpression { get; set; }
        internal StartExpression StartExpression { get; set; }
        internal IssueParameters Parameters { get; set; }
        internal AnalyticalExpression AnalyticalExpression { get; set; }
    }
}
