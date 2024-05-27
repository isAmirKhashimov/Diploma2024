namespace Diploma3.JsonModels
{
	public class TPoint (int pointNumber, double x, double y)
    {
        public int PointNumber { get; set; } = pointNumber;
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
    }
}
