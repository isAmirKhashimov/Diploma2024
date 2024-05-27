using System.Windows;

namespace Diploma3
{
	public class PointGenerator(List<ReferencePoint> borderPoints, double minimalDistance, double bandWidth)
	{
		public static double MinimalDistanceRelaxer = 0.65;

		private readonly double leftX = borderPoints.Min(p => p!.Value.X);
		private readonly double rightX = borderPoints.Max(p => p!.Value.X);
		private readonly double leftY = borderPoints.Min(p => p!.Value.Y);
		private readonly double rightY = borderPoints.Max(p => p!.Value.Y);
		private readonly Random random = new Random();

		public List<Point> Generate()
		{
			var res = new List<Point>();

			for (double x = leftX + minimalDistance; x <= rightX - minimalDistance; x += bandWidth + minimalDistance)
			{
				for (double y = leftY + minimalDistance; y <= rightY - minimalDistance; y += bandWidth + minimalDistance)
				{
					var xValue = NextDouble(x, x + bandWidth);
					var yValue = NextDouble(y, y + bandWidth);
					if (InsideTheBorder(xValue, yValue))
					{
						res.Add(new(xValue, yValue));
					}
				}
			}

			return res;
		}

		private double NextDouble(double l, double r) => l + (r - l) * random.NextDouble();


		public static bool InsideTheBorder(List<ReferencePoint> borderPoints, double minimalDistance, double x, double y)
		{
			minimalDistance *= MinimalDistanceRelaxer;

			int npol = borderPoints.Count;
			var xp = borderPoints.Select(el => el.Value.X).ToArray();
			var yp = borderPoints.Select(el => el.Value.Y).ToArray();

			var res = true;
			res = res && InsideTheBorderD(x - minimalDistance, y - minimalDistance);
			res = res && InsideTheBorderD(x, y - minimalDistance);
			res = res && InsideTheBorderD(x + minimalDistance, y - minimalDistance);
			res = res && InsideTheBorderD(x - minimalDistance, y);
			res = res && InsideTheBorderD(x, y);
			res = res && InsideTheBorderD(x + minimalDistance, y);
			res = res && InsideTheBorderD(x - minimalDistance, y + minimalDistance);
			res = res && InsideTheBorderD(x, y + minimalDistance);
			res = res && InsideTheBorderD(x + minimalDistance, y + minimalDistance);

			return res;

			bool InsideTheBorderD(double xd, double yd)
			{

				bool c = false;
				for (int i = 0, j = npol - 1; i < npol; j = i++)
				{
					if ((
					  (yp[i] < yp[j]) && (yp[i] <= yd) && (yd <= yp[j]) &&
					  ((yp[j] - yp[i]) * (xd - xp[i]) > (xp[j] - xp[i]) * (yd - yp[i]))
					) || (
					  (yp[i] > yp[j]) && (yp[j] <= yd) && (yd <= yp[i]) &&
					  ((yp[j] - yp[i]) * (xd - xp[i]) < (xp[j] - xp[i]) * (yd - yp[i]))
					))
						c = !c;
				}
				return c;
			}
		}

		private bool InsideTheBorder(double x, double y) => InsideTheBorder(borderPoints, minimalDistance, x, y);
	}
}