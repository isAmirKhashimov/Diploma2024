namespace Delaune
{
	public interface ITriangle
	{
		IEnumerable<IPoint> Points { get; }
		int Index { get; }
	}

}
