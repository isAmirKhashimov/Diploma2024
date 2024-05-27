namespace Diploma3
{
	internal class FiniteElement
	{
		private readonly IssueParameters issueParameters;
		private readonly double square;

		public FiniteElement(Node nodeI, Node nodeJ, Node nodeK, IssueParameters issueParameters)
		{
			NodeI = nodeI;
			NodeJ = nodeJ;
			NodeK = nodeK;

			this.issueParameters = issueParameters;

			// Возможно нужен Math.Abs, но выяснится позже.
			square = 0.5 * 
				((NodeJ.X * NodeK.Y - NodeK.X * NodeJ.Y) -
				(NodeI.X * NodeK.Y - NodeK.X * NodeI.Y) +
				(NodeI.X * NodeJ.Y - NodeJ.X * NodeI.Y));

			Ni = new BaseFunction(nodeI, nodeJ, nodeK, square, NodeIndex.I);
			Nj = new BaseFunction(nodeI, nodeJ, nodeK, square, NodeIndex.J);
			Nk = new BaseFunction(nodeI, nodeJ, nodeK, square, NodeIndex.K);
		}
		
		public BaseFunction Ni { get; }
		public BaseFunction Nj { get; }
		public BaseFunction Nk { get; }

		public Node NodeI { get; }
		public Node NodeJ { get; }
		public Node NodeK { get; }

		public double[,] GetA()
		{
			var xSr = GetMeanX(NodeI, NodeJ, NodeK);
			var ySr = GetMeanY(NodeI, NodeJ, NodeK);

			var aKoef = issueParameters.MainEquation.Lambda(xSr, ySr) * square;

			var res = new double[issueParameters.M, issueParameters.M];

			res[NodeI.Number, NodeI.Number] = aKoef * (Ni.Beta * Ni.Beta + Ni.Gamma * Ni.Gamma);
			res[NodeI.Number, NodeJ.Number] = aKoef * (Ni.Beta * Nj.Beta + Ni.Gamma * Nj.Gamma);
			res[NodeI.Number, NodeK.Number] = aKoef * (Ni.Beta * Nk.Beta + Ni.Gamma * Nk.Gamma);

			res[NodeJ.Number, NodeI.Number] = aKoef * (Nj.Beta * Ni.Beta + Nj.Gamma * Ni.Gamma);
			res[NodeJ.Number, NodeJ.Number] = aKoef * (Nj.Beta * Nj.Beta + Nj.Gamma * Nj.Gamma);
			res[NodeJ.Number, NodeK.Number] = aKoef * (Nj.Beta * Nk.Beta + Nj.Gamma * Nk.Gamma);

			res[NodeK.Number, NodeI.Number] = aKoef * (Nk.Beta * Ni.Beta + Nk.Gamma * Ni.Gamma);
			res[NodeK.Number, NodeJ.Number] = aKoef * (Nk.Beta * Nj.Beta + Nk.Gamma * Nj.Gamma);
			res[NodeK.Number, NodeK.Number] = aKoef * (Nk.Beta * Nk.Beta + Nk.Gamma * Nk.Gamma);

			if (NodeI.IsOnBorder && NodeJ.IsOnBorder)
			{
				var distance = GetDistance(NodeI, NodeJ);

				res[NodeI.Number, NodeI.Number] -= distance / 3.0;
				res[NodeI.Number, NodeJ.Number] -= distance / 6.0;
															   
				res[NodeJ.Number, NodeI.Number] -= distance / 6.0;
				res[NodeJ.Number, NodeJ.Number] -= distance / 3.0;


				var teta = Math.Atan2(NodeJ.Y - NodeI.Y, NodeJ.X - NodeI.X);
				var xdSr = GetMeanX(NodeI, NodeJ);
				var ydSr = GetMeanY(NodeI, NodeJ);
				var cos_in = -Math.Sin(teta);
				var cos_jn = Math.Cos(teta);

				var lambdaKoef = 0.5 * issueParameters.MainEquation.Lambda(xdSr, ydSr) * distance;

				// ???????? //
				res[NodeI.Number, NodeI.Number] -= lambdaKoef * (cos_in * Ni.Beta + cos_jn * Ni.Gamma);
				res[NodeI.Number, NodeJ.Number] -= lambdaKoef * (cos_in * Nj.Beta + cos_jn * Nj.Gamma);
				res[NodeI.Number, NodeK.Number] -= lambdaKoef * (cos_in * Nk.Beta + cos_jn * Nk.Gamma);

				res[NodeJ.Number, NodeI.Number] -= lambdaKoef * (cos_in * Ni.Beta + cos_jn * Ni.Gamma);
				res[NodeJ.Number, NodeJ.Number] -= lambdaKoef * (cos_in * Nj.Beta + cos_jn * Nj.Gamma);
				res[NodeJ.Number, NodeK.Number] -= lambdaKoef * (cos_in * Nk.Beta + cos_jn * Nk.Gamma);
			}
			else if (NodeI.IsOnBorder && NodeK.IsOnBorder)
			{
				var distance = GetDistance(NodeI, NodeK);

				res[NodeI.Number, NodeI.Number] -= distance / 3.0;
				res[NodeI.Number, NodeK.Number] -= distance / 6.0;

				res[NodeK.Number, NodeI.Number] -= distance / 6.0;
				res[NodeK.Number, NodeK.Number] -= distance / 3.0;


				var teta = Math.Atan2(NodeI.Y - NodeK.Y, NodeI.X - NodeK.X);
				var xdSr = GetMeanX(NodeK, NodeI);
				var ydSr = GetMeanY(NodeK, NodeI);
				var cos_in = -Math.Sin(teta);
				var cos_jn = Math.Cos(teta);

				var lambdaKoef = 0.5 * issueParameters.MainEquation.Lambda(xdSr, ydSr) * distance;

				// ???????? //
				res[NodeK.Number, NodeI.Number] -= lambdaKoef * (cos_in * Ni.Beta + cos_jn * Ni.Gamma);
				res[NodeK.Number, NodeJ.Number] -= lambdaKoef * (cos_in * Nj.Beta + cos_jn * Nj.Gamma);
				res[NodeK.Number, NodeK.Number] -= lambdaKoef * (cos_in * Nk.Beta + cos_jn * Nk.Gamma);

				res[NodeI.Number, NodeI.Number] -= lambdaKoef * (cos_in * Ni.Beta + cos_jn * Ni.Gamma);
				res[NodeI.Number, NodeJ.Number] -= lambdaKoef * (cos_in * Nj.Beta + cos_jn * Nj.Gamma);
				res[NodeI.Number, NodeK.Number] -= lambdaKoef * (cos_in * Nk.Beta + cos_jn * Nk.Gamma);
			}
			else if (NodeJ.IsOnBorder && NodeK.IsOnBorder)
			{
				var distance = GetDistance(NodeJ, NodeK);

				res[NodeJ.Number, NodeJ.Number] -= distance / 3.0;
				res[NodeJ.Number, NodeK.Number] -= distance / 6.0;
												
				res[NodeK.Number, NodeJ.Number] -= distance / 6.0;
				res[NodeK.Number, NodeK.Number] -= distance / 3.0;


				var teta = Math.Atan2(NodeK.Y - NodeJ.Y, NodeK.X - NodeJ.X);
				var xdSr = GetMeanX(NodeJ, NodeK);
				var ydSr = GetMeanY(NodeJ, NodeK);
				var cos_in = -Math.Sin(teta);
				var cos_jn = Math.Cos(teta);

				var lambdaKoef = 0.5 * issueParameters.MainEquation.Lambda(xdSr, ydSr) * distance;

				// ???????? //
				res[NodeJ.Number, NodeI.Number] -= lambdaKoef * (cos_in * Ni.Beta + cos_jn * Ni.Gamma);
				res[NodeJ.Number, NodeJ.Number] -= lambdaKoef * (cos_in * Nj.Beta + cos_jn * Nj.Gamma);
				res[NodeJ.Number, NodeK.Number] -= lambdaKoef * (cos_in * Nk.Beta + cos_jn * Nk.Gamma);
												
				res[NodeK.Number, NodeI.Number] -= lambdaKoef * (cos_in * Ni.Beta + cos_jn * Ni.Gamma);
				res[NodeK.Number, NodeJ.Number] -= lambdaKoef * (cos_in * Nj.Beta + cos_jn * Nj.Gamma);
				res[NodeK.Number, NodeK.Number] -= lambdaKoef * (cos_in * Nk.Beta + cos_jn * Nk.Gamma);
			}

			return res;
		}

		public double[] GetF()
		{
			var xSr = GetMeanX(NodeI, NodeJ, NodeK);
			var ySr = GetMeanY(NodeI, NodeJ, NodeK);

			var fKoef = -issueParameters.MainEquation.F(xSr, ySr) * square / 3.0;
			
			var res = new double[issueParameters.M];

			res[NodeI.Number] = fKoef;
			res[NodeJ.Number] = fKoef;
			res[NodeK.Number] = fKoef;

			if (NodeI.IsOnBorder && NodeJ.IsOnBorder && NodeK.IsOnBorder)
			{
				throw new Exception("Три точки одного треугольника не должны лежать на границе области.");
			}

			if (NodeI.IsOnBorder && NodeJ.IsOnBorder)
			{
				var xdSr = GetMeanX(NodeI, NodeJ);
				var ydSr = GetMeanY(NodeI, NodeJ);
				var fiKoef = issueParameters.BorderEquation.Fi(xdSr, ydSr) * GetDistance(NodeI, NodeJ) / 2.0;

				res[NodeI.Number] -= fiKoef;
				res[NodeJ.Number] -= fiKoef;
			}
			else if (NodeI.IsOnBorder && NodeK.IsOnBorder)
			{
				var xdSr = GetMeanX(NodeI, NodeK);
				var ydSr = GetMeanY(NodeI, NodeK);
				var fiKoef = issueParameters.BorderEquation.Fi(xdSr, ydSr) * GetDistance(NodeI, NodeK) / 2.0;

				res[NodeI.Number] -= fiKoef;
				res[NodeK.Number] -= fiKoef;
			}
			else if (NodeJ.IsOnBorder && NodeK.IsOnBorder)
			{
				var xdSr = GetMeanX(NodeJ, NodeK);
				var ydSr = GetMeanY(NodeJ, NodeK);
				var fiKoef = issueParameters.BorderEquation.Fi(xdSr, ydSr) * GetDistance(NodeJ, NodeK) / 2.0;

				res[NodeJ.Number] -= fiKoef;
				res[NodeK.Number] -= fiKoef;
			}

			return res;
		}

		private static double GetDistance(Node a, Node b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

		private static double GetMeanX(params Node[] nodes) => nodes.Sum(node => node.X) / nodes.Length;
		
		private static double GetMeanY(params Node[] nodes) => nodes.Sum(node => node.Y) / nodes.Length;
	}
}
