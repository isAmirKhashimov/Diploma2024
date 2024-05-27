namespace Diploma3
{
	internal readonly record struct Node(double X, double Y, int Number, bool IsOnBorder)
	{
		public bool IsOnBorder { get; } = IsOnBorder;
		public int Number { get; } = Number;
		public double X { get; } = X;
		public double Y { get; } = Y;
	}
}
