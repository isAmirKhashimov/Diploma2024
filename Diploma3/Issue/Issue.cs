namespace Diploma3
{
	internal class Issue
    {
        public required IssueParameters Parameters { get; init; }

		public List<FiniteElement> FiniteElements { get; private set; } = [];

        public List<Node> Nodes { get; private set; } = [];

		public required Func<double, double, double> AnalyticalResult { get; init; }

		public double[] Solve()
        {
            var a = ArrayUtils.SumSqMatrixes(FiniteElements.Select(el => el.GetA()).ToArray());
            var f = ArrayUtils.SumVectors(FiniteElements.Select(el => el.GetF()).ToArray());

         /*   var system = new GaussMethod(a, f);
            if (system.SolveMatrix() == 0)
            {
                return system.Answer;
            }
            else
            {
                throw new Exception();
            }*/
            var system = new LinearSystem(a, f);
            return system.SolveLib();
        }

        public void UpdateFiniteElemetns(List<Node> nodes, List<FiniteElement> finiteElements)
        {
            Nodes = nodes;
            FiniteElements = finiteElements;
        }
    }
}