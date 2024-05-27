using WPoint = System.Windows.Point;

namespace Diploma3.ContoureDetection
{
    public sealed class Contoures
    {
		private readonly System.Drawing.Point[][] contoures;
        
        public Contoures(System.Drawing.Point[][] contoures)
        {
			this.contoures = [.. contoures.OrderByDescending(arr => arr.Length)];
        }

		public int Count => contoures.Length;

		public IEnumerable<WPoint> GetContoure(int ContoureIndex, int countLimit, int approximationDensity, double eps, double distanceLimit, double refineDensity, double closestDistance) => contoures.Any() ? RefineLines(FilirateLines(contoures[ContoureIndex].Select(el => el.ToPoint()).Where((_, index) => contoures[ContoureIndex].Length < countLimit || index % approximationDensity == 0), eps, distanceLimit, closestDistance), refineDensity) : Enumerable.Empty<WPoint>();
		/*
        public void AddCurrentIndexOffset(int offset)
        {
            currentIndex += offset;
            currentIndex = Math.Min(currentIndex, contoures.Length - 1);
            currentIndex = Math.Max(currentIndex, 0);
        }
        public void AddApproximationDensityOffset(int offset)
        {
            approximationDensity += offset;
            approximationDensity = Math.Min(approximationDensity, contoures[currentIndex].Length / 3);
            approximationDensity = Math.Max(approximationDensity, 5);
        }
		*/

		/*
		public static IEnumerable<System.Windows.Point> TakeByDensity(IEnumerable<System.Windows.Point> points)
		{
			
		}*/


		public static List<WPoint> RemoveClosest(IEnumerable<System.Windows.Point> borderPoints, double closestDistance)
		{
			if (!borderPoints.Any()) return [];

			var res = new List<System.Windows.Point>
			{
				borderPoints.First()
			};

			foreach (var point in borderPoints)
			{
				if (GetDistance(point, res.Last()) >= closestDistance) res.Add(point);
			}

			return res;
		}

		private static List<WPoint> FilirateLines(IEnumerable<WPoint> borderPoints, double eps, double distance, double closestDistance)
		{
			var points = RemoveClosest(borderPoints, closestDistance);
			int prevCount;

			do
			{
				prevCount = points.Count;
				int ind1 = 0;

				while (true && points.Count > 0)
				{
					var ind2 = (ind1 + 1) % points.Count;
					var ind3 = (ind1 + 2) % points.Count;

					if (ApproximatelyOnTheLine(points[ind1], points[ind2], points[ind3], eps, distance))
					{
						points.RemoveAt(ind2);
						if (ind2 == 0) ind1--;
					}
					else
					{
						ind1 = ind2;
						if (ind1 == 0) break;
					}
				}

			} while (points.Count > 4 && points.Count != prevCount);
			

			return points;
		}

		private static IEnumerable<WPoint> RefineLines(IEnumerable<WPoint> borderPoints, double density)
		{
			if (!borderPoints.Any()) yield break;

			var previousPoint = borderPoints.Last(); 
			foreach (var borderPoint in borderPoints)
			{
				var distance = GetDistance(borderPoint, previousPoint);
				var partsCount = (int)Math.Round(distance / density);
				var deltaX = (borderPoint.X - previousPoint.X) / partsCount;
				var deltaY = (borderPoint.Y - previousPoint.Y) / partsCount;

				yield return previousPoint;
				for (int i = 1; i < partsCount; i++)
				{
					yield return new WPoint(previousPoint.X + deltaX * i, previousPoint.Y + deltaY * i);
				}
				yield return borderPoint;

				previousPoint = borderPoint;
			}

			yield break;
		}

		private static bool ApproximatelyOnTheLine(WPoint a, WPoint mid, WPoint b, double eps, double distance)
		{
			var midbk = Math.Atan2(b.Y - mid.Y, b.X - mid.X);
			var abk = Math.Atan2(b.Y - a.Y, b.X - a.X);
			var amidk = Math.Atan2(mid.Y - a.Y, mid.X - a.X);

			var bmdist = GetDistance(b, mid);
			var amdist = GetDistance(a, mid);

			return (Math.Abs(abk - amidk) < eps && Math.Abs(abk - midbk) < eps) || amdist < distance || bmdist < distance;
		}

		private static double GetDistance(WPoint a, WPoint b)
		{
			return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
		}
	}
}