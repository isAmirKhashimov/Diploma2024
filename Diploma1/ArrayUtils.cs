namespace Diploma1
{
    internal static class ArrayUtils
	{
        public static double[,] BuildSquareMatrix(int n, Func<int, int, double> f)
        {
            var res = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    res[i, j] = f(i, j);
                }
            }

            return res;
        }

		public static double[] BuildVector(int n, Func<int, double> f)
		{
			var res = new double[n];

			for (int i = 0; i < n; i++)
			{
                res[i] = f(i);
			}

			return res;
		}

		public static double[] GetDerivative(double[] xs, double[] ys)
		{
			if (xs.Length != ys.Length) throw new ArgumentException();

			var h = xs[1] - xs[0];
			var n = xs.Length - 1;
			var res = new double[n + 1];

			res[0] = (-3.0 * ys[0] + 4.0 * ys[1] - ys[2]) / (2.0 * h);
			res[n] = (3.0 * ys[n] - 4.0 * ys[n - 1] + ys[n - 2]) / (2.0 * h);

			for (int i = 1; i < n; i++)
			{
				res[i] = (ys[i + 1] - ys[i - 1]) / (2.0 * h);
			}

			return res;
		}

        // методом трапеций
		public static double GetIntegral(double[] ys, double a, double b)
        {
            var n = ys.Length - 1;
            var dx = (b - a) / n;
            return (ys[1..n].Sum() + (ys[0] + ys[n]) / 2.0) * dx;
        }

        public static double[] FillLike(double[] arr, double value) => Enumerable.Range(0, arr.Length).Select(_ => value).ToArray();

        public static double[] Linspace(double min, double max, int count) => Enumerable.Range(0, count).Select(el => el * (max - min) / count + min).ToArray();
        
        public static double[] Reshape(double[] arr, int count) => Linspace(arr.First(), arr.Last(), count);
        
        //public static double[] Reshape(double[,] arr, int countX, int countY) => Linspace(arr.First(), arr.Last(), count);
        
        public static double[] ZerosLike(double[] arr) => new double[arr.Length];
        
        public static double[] Zeros(int count) => new double[count];


        public static double[] Multiply(double[,] matrixA, double[] vectorB) => Multiply(matrixA, vectorB.VectorToMatrix()).MatrixToVector();

        public static double[,] Multiply(double[,] matrixA, double[,] matrixB)
        {
            if (matrixA.ColumnsCount() != matrixB.RowsCount())
            {
                throw new Exception("Умножение не возможно! Количество столбцов первой матрицы не равно количеству строк второй матрицы.");
            }

            var matrixC = new double[matrixA.RowsCount(), matrixB.ColumnsCount()];

            for (var i = 0; i < matrixA.RowsCount(); i++)
            {
                for (var j = 0; j < matrixB.ColumnsCount(); j++)
                {
                    matrixC[i, j] = 0;

                    for (var k = 0; k < matrixA.ColumnsCount(); k++)
                    {
                        matrixC[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }

            return matrixC;
        }

        public static double[] AddByElements(double[] first, double[] second) => ApplyOperationByElements(first, second, (zip) => zip.Item1 + zip.Item2);
        
        public static double[] DivideByElements(double[] first, double number) => ApplyOperationByElements(first, FillLike(first, number), (zip) => zip.Item1 / zip.Item2);
        
        public static double[] DivideByElements(double[] first, double[] second) => ApplyOperationByElements(first, second, (zip) => zip.Item1 / zip.Item2);

        public static double[] MultiplyByElements(double[] first, double[] second) => ApplyOperationByElements(first, second, (zip) => zip.Item1 * zip.Item2);

        public static double[] SubtractByElements(double[] first, double[] second) => ApplyOperationByElements(first, second, (zip) => zip.Item1 - zip.Item2);

        private static T[] ApplyOperationByElements<T>(T[] first, T[] second, Func<(T, T), T> operation)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            if (first.Length != second.Length) throw new ArgumentException();

            return first.Zip(second).Select(operation).ToArray();
        }
    }
}
