namespace Diploma2
{
	internal class Program
	{
		public const int M = 3;
		public const int a = 0;
		public const int b = 1;
		public const double q0 = 0;
		public const double q1 = 1;
		
		public static double[] xs;
		
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
			xs = ArrayUtils.LinspaceIn(a, b, M);
			var ks = GetK();
			var fs = new double[M + 1];

			var system = new LinearSystem(ks, fs);
			// Порядок важен
			system.RecalcMAsConstant(M, q1);
			system.RecalcMAsConstant(0, q0);
			var res = system.Solve();
			res = [q0, .. res, q1];
			Console.WriteLine(string.Join(" ", res));
		}
		
		public static double[,] GetK()
		{
			var res = new double[M + 1, M + 1];

			for (int m = 0; m < M; m++)
			{
				AddKsmTo(res, m);
			}

			return res;
		}

		public static void AddKsmTo(double[,] matrix, int m)
		{
			matrix[m, m] += GetIntegralForMS(m, m);
			matrix[m, m + 1] += GetIntegralForMS(m, m + 1);
			matrix[m + 1, m] += GetIntegralForMS(m + 1, m);
			matrix[m + 1, m + 1] += GetIntegralForMS(m + 1, m + 1);

			
			double GetIntegralForMS(int mm, int ss)
			{
				return (GetIntegralIn(xs[m + 1]) - GetIntegralIn(xs[m])) * (mm == ss ? 1 : -1);


				double GetIntegralIn(double x)
				{
					return (M * M) *
						(
							x * x * x / 3.0
							- x * x * (xs[mm] + xs[ss]) / 2.0
							+ x * (xs[mm] * xs[ss] + 1)
						);
				}
			}
		}
	}
}
