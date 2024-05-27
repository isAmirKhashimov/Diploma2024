using System.Drawing;

namespace Diploma3
{
	internal class Controller
    {
        private const double eps = 0.00000001;
        private const int n = 50;
        private const double xl = 0;
        private const double yl = 0;
        private const double xr = 1;
        private static double yr = 1;

        public double[,] Result { get; private set; }
        public double[,] AnalyticalResult { get; private set; }

        /*
         * ddx + ddy = sin pi*x + sin pi*y
        - pi2 (sin pi x + sin pi2 y)


        u = sin pi*x + sin pi*y
        u'(0, 0) = pi * cos(pi * x) + cos
        -pi -pi 
*/

        public double[] RangeOfX => Enumerable.Range(0, Issue.Parameters.Nx + 1).Select(x => Issue.Parameters.Xl + Issue.Parameters.Hx * x).ToArray();
        public double[] RangeOfY => Enumerable.Range(0, Issue.Parameters.Ny + 1).Select(x => Issue.Parameters.Yl + Issue.Parameters.Hy * x).ToArray();
		
        // u = (x - 0.5)^2 + (y - 0.5)^2  
        public Issue Issue { get; } = new ()
        {
            AnalyticalResult = (x, y) => (x - 0.5) * (x - 0.5) + (y - 0.5) * (y - 0.5),
			Parameters = new IssueParameters()
            {
                M = 5,
                MainEquation = new MainEquation((x, y) => 1, (x, y) => 2 * (x + y - 1)),
                BorderEquation = new BorderEquation(1, (x, y) =>
                {
                    if (EqualApproximately(x, xl)) return (y - 0.5) * (y - 0.5) + 0.25;
					if (EqualApproximately(x, xr)) return (y - 0.5) * (y - 0.5) + 0.25;
					if (EqualApproximately(y, yl)) return (x - 0.5) * (x - 0.5) + 0.25;
					if (EqualApproximately(y, yr)) return (x - 0.5) * (x - 0.5) + 0.25;
					throw new Exception();
                }),
                Nx = n,
                Ny = n,
                Hx = (xr - xl) / n, 
                Hy = (yr - yl) / n,
                Xl = xl,
                Yl = yl,
                Xr = xr,
                Yr = yr,
            },
        };



		public void Calculate()
        {
			
            List<Node> nodes =
            [
                new(xl, yl, 0, true),
                new(xr, yl, 1, true),
                new((xl + xr) / 2, (yl + yr) / 2, 2, false),
                new(xl, yr, 3, true),
                new(xr, yr, 4, true),
            ];

            List<FiniteElement> finiteElements =
            [
                new(nodes[0], nodes[1], nodes[2], Issue.Parameters),
                new(nodes[1], nodes[4], nodes[2], Issue.Parameters),
                new(nodes[2], nodes[4], nodes[3], Issue.Parameters),
                new(nodes[0], nodes[2], nodes[3], Issue.Parameters),
            ];
            /*
			List<Node> nodes =
			[
				new(xl4(0), yl4(0), 0, true),
				new(xl4(1), yl4(1), 1, false),
				new(xl4(2), yl4(0), 2, true),
				new(xl4(4), yl4(0), 3, true),
				new(xl4(3), yl4(1), 4, false),
				new(xl4(4), yl4(2), 5, true),
				new(xl4(2), yl4(2), 6, false),
				new(xl4(0), yl4(2), 7, true),
				new(xl4(1), yl4(3), 8, false),
				new(xl4(0), yl4(4), 9, true),
				new(xl4(2), yl4(4), 10, true),
				new(xl4(3), yl4(3), 11, false),
				new(xl4(4), yl4(4), 12, true),
			];

			List<FiniteElement> finiteElements =
			[
				Fin(0, 2, 1),
				Fin(0, 1, 7),
				Fin(1, 2, 6),
				Fin(1, 6, 7),

				Fin(2, 3, 4),
				Fin(3, 5, 4),
				Fin(6, 4, 5),
				Fin(2, 4, 6),

				Fin(7, 6, 8),
				Fin(7, 8, 9),
				Fin(9, 8, 10),
				Fin(8, 6, 10),

				Fin(6, 11, 10),
				Fin(6, 5, 11),
				Fin(5, 12, 11),
				Fin(10, 11, 12),
			];
            */
			double xl4(int p) => xl + p / 4.0 * (xr - xl);
			double yl4(int p) => yl + p / 4.0 * (yr - yl);
			FiniteElement Fin(int i, int j, int k) => new(nodes[i], nodes[j], nodes[k], Issue.Parameters);

            Calculate(nodes, finiteElements);
		}

        /*
        public void Calculate(List<(Point Point, bool IsOnBorder)> points, List<(int PointI, int PointJ, int PointK)> triangles)
        {

        }
        */

		public void Calculate(List<Node> nodes, List<FiniteElement> finiteElements)
        {
			Issue.UpdateFiniteElemetns(nodes, finiteElements);

            var koefs = Issue.Solve();

            var rangeOfX = RangeOfX;
            var rangeOfY = RangeOfY;

            Result = new double[Issue.Parameters.Ny, Issue.Parameters.Nx];
            AnalyticalResult = new double[Issue.Parameters.Ny, Issue.Parameters.Nx];

			/*
             * (x1 - x0) * (y2 - y1) - (x2 - x1) * (y1 - y0)
             * (x2 - x0) * (y3 - y2) - (x3 - x2) * (y2 - y0)
             * (x3 - x0) * (y1 - y3) - (x1 - x3) * (y3 - y0)
             */

			for (int yIndex = 0; yIndex < Issue.Parameters.Ny; yIndex++)
            {
                for (int xIndex = 0; xIndex < Issue.Parameters.Nx; xIndex++)
                {
                    var y = rangeOfY[yIndex];
                    var x = rangeOfX[xIndex];

					var el = finiteElements.First(el => IsInTriangle(el, x, y));

					Result[yIndex, xIndex]
						= koefs[el.NodeI.Number] * el.Ni.OfXY(x, y)
						+ koefs[el.NodeJ.Number] * el.Nj.OfXY(x, y)
						+ koefs[el.NodeK.Number] * el.Nk.OfXY(x, y);

					AnalyticalResult[yIndex, xIndex] = Issue.AnalyticalResult(x, y);
				}
            }

			static bool IsInTriangle(FiniteElement finiteElement, double x, double y)
            {
                var n1 = finiteElement.NodeI;
                var n2 = finiteElement.NodeJ;
                var n3 = finiteElement.NodeK;


				if (EqualApproximately(x, n1.X)) x = n1.X;
				if (EqualApproximately(x, n2.X)) x = n2.X;
				if (EqualApproximately(x, n3.X)) x = n3.X;
				if (EqualApproximately(y, n1.Y)) y = n1.Y;
				if (EqualApproximately(y, n2.Y)) y = n2.Y;
				if (EqualApproximately(y, n3.Y)) y = n3.Y;

				var a = (n1.X - x) * (n2.Y - n1.Y) - (n2.X - n1.X) * (n1.Y - y);
                var b = (n2.X - x) * (n3.Y - n2.Y) - (n3.X - n2.X) * (n2.Y - y);
                var c = (n3.X - x) * (n1.Y - n3.Y) - (n1.X - n3.X) * (n3.Y - y);

                if (EqualApproximately(a, 0)) a = 0;
                if (EqualApproximately(b, 0)) b = 0;
                if (EqualApproximately(c, 0)) c = 0;

				return (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
			}
        }
		private static bool EqualApproximately(double a, double b) => Math.Abs(a - b) < eps;
	}
}
