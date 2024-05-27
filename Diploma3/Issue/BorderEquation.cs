using FuncR2 = System.Func<double, double, double>;

namespace Diploma3
{
	internal class BorderEquation(double alpha, FuncR2 fi)
	{
		public double Alpha { get; } = alpha;

		public FuncR2 Fi { get; } = fi ?? throw new ArgumentNullException(nameof(fi));
	}
}
