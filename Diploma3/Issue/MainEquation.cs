using FuncR2 = System.Func<double, double, double>;

namespace Diploma3
{
	internal class MainEquation(FuncR2 lambda, FuncR2 f)
	{
		public FuncR2 Lambda { get; } = lambda ?? throw new ArgumentNullException(nameof(lambda));

		public FuncR2 F { get; } = f ?? throw new ArgumentNullException(nameof(f));
	}
}
