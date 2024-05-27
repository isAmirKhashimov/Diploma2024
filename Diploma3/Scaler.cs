using System.Windows;

namespace Diploma3
{
	public struct Scaler()
	{
		public double MultiplierX { get; set; } = 0.002;
		public double MultiplierY { get; set; } = -0.002;
		public double ShiftX { get; set; } = -0.28;
		public double ShiftY { get; set; } = 1.08;

		public readonly Point Transform(Point p) => new(p.X * MultiplierX + ShiftX, p.Y * MultiplierY + ShiftY);
		public readonly Point TransformBack(Point p) => new((p.X - ShiftX) / MultiplierX, (p.Y - ShiftY) / MultiplierY);
		public readonly double TransformBack(double t) => t / MultiplierX;
	}
}