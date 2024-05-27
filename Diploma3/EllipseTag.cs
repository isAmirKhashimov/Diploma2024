using DPoint = Delaune.Point;

namespace Diploma3
{
	public partial class MainWindow
	{
		private readonly struct EllipseTag(bool isOnBorder, DPoint point)
		{
			public bool IsOnBorder { get; } = isOnBorder;
			public DPoint Point { get; } = point;
		}
	}
}