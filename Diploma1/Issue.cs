using FuncR = System.Func<double, double>;

namespace Diploma1
{
	internal class Issue
    {
        public required Function K { get; init; }
        public required Function P { get; init; }
        public required Function Q { get; init; }

		public required Function[] Fis { get; init; }

        public required IssueParameters Parameters { get; init; }



        public double[] Solve()
        {
            var G = ArrayUtils.BuildSquareMatrix(Parameters.PolynomsCount, (i, k) => GetDotProduct2(Fis[i], Fis[k]));
            var b = ArrayUtils.BuildVector(Parameters.PolynomsCount, (i) => -GetDotProduct1(Fis[i], Q));

            var system = new LinearSystem(G, b);
            return system.Solve();
        }

        public double GetDotProduct1(Function fi, Function psi)
        {
            var f = ArrayUtils.MultiplyByElements(fi.U, psi.U);
            return ArrayUtils.GetIntegral(f, Parameters.LeftX, Parameters.RightX);
        }

		public double GetDotProduct2(Function fi, Function psi)
		{
			var fLeft = ArrayUtils.MultiplyByElements(K.U, fi.U_1);
            fLeft = ArrayUtils.MultiplyByElements(fLeft, psi.U_1);

			var fRight= ArrayUtils.MultiplyByElements(P.U, fi.U);
			fRight = ArrayUtils.MultiplyByElements(fRight, psi.U);

            var f = ArrayUtils.AddByElements(fLeft, fRight);

			return ArrayUtils.GetIntegral(f, Parameters.LeftX, Parameters.RightX);
		}
	}


    internal class Function
    {
		private readonly double left;
		private readonly double right;
		private readonly int count;


		public Function(FuncR functionInstance, double left, double right, int count)
        {
            Instance = functionInstance;
			this.left = left;
			this.right = right;
			this.count = count;

            var xs = ArrayUtils.Linspace(left, right, count);
            U = xs.Select(x => Instance(x)).ToArray();
            U_1 = ArrayUtils.GetDerivative(xs, U);
        }

        public FuncR Instance { get; }

        public double[] U { get; }

        public double[] U_1 { get; }
    }

    internal interface IPolynomProvider
    {
        const int LimitN = 1000000;
        IEnumerable<Func<double, double>> GetPolynoms();
    }

	internal class Legendre : IPolynomProvider
	{
        public IEnumerable<FuncR> GetPolynoms()
        {

            return new List<FuncR>()
            {
                _ => 1,
                x => x,
                x => 0.5 * (3 * x * x - 1),
                x => 0.5 * (5 * x * x * x - 3 * x),
            };
        }
	}
}
