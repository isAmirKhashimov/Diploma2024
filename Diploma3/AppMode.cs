namespace Diploma3
{
	public partial class MainWindow
	{
		[Flags]
		private enum AppMode
		{
			Scaling = 1,
			Bordering = 2,
			Triangulation = 4,
		}
	}
}