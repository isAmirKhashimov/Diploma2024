namespace Diploma3
{
	public class CanvasInnerPoint(ReferencePoint point, bool isOnBorder)
	{
		public ReferencePoint Point { get; } = point;
		public bool IsOnBorder { get; } = isOnBorder;

		public override bool Equals(object? obj)
		{
			if (obj is not CanvasInnerPoint other) return false;
			if (Point.Value != other.Point.Value) return false;
			return true;
			return Point == other.Point && IsOnBorder == other.IsOnBorder ? true : throw new Exception();
		}

		public override int GetHashCode()
		{
			return Point.Value.GetHashCode() * 2 + (IsOnBorder ? 1 : 0);
		}
	}
}