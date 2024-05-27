namespace Diploma3
{
	internal class BaseFunction
	{
		public BaseFunction(Node nodeI, Node nodeJ, Node nodeK, double elementSquare, NodeIndex firstNode)
		{
			switch (firstNode)
			{
				case NodeIndex.I:
					Alpha = 1.0 / (2.0 * elementSquare) * (nodeJ.X * nodeK.Y - nodeK.X * nodeJ.Y);
					Beta = 1.0 / (2.0 * elementSquare) * (nodeJ.Y - nodeK.Y);
					Gamma = 1.0 / (2.0 * elementSquare) * (nodeK.X - nodeJ.X);
					break;

				case NodeIndex.J:
					Beta = 1.0 / (2.0 * elementSquare) * (nodeK.X * nodeI.Y - nodeI.X * nodeK.Y);
					Gamma = 1.0 / (2.0 * elementSquare) * (nodeK.Y - nodeI.Y);
					Alpha = 1.0 / (2.0 * elementSquare) * (nodeI.X - nodeK.X);
					break;

				case NodeIndex.K:
					Gamma = 1.0 / (2.0 * elementSquare) * (nodeI.X * nodeJ.Y - nodeJ.X * nodeI.Y);
					Alpha = 1.0 / (2.0 * elementSquare) * (nodeI.Y - nodeJ.Y);
					Beta = 1.0 / (2.0 * elementSquare) * (nodeJ.X - nodeI.X);
					break;

				default:
					throw new NotImplementedException();
			}
		}

		public double Alpha { get; }
		
		public double Beta { get; }

		public double Gamma { get; }

		public double OfXY(double x, double y) => Alpha + Beta * x + Gamma * y;
	}
}
