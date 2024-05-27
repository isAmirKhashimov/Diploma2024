namespace Diploma1
{
    static class MatrixExt
    {
        // метод расширения для получения количества строк матрицы
        public static int RowsCount(this double[,] matrix)
        {
            return matrix.GetUpperBound(0) + 1;
        }

        // метод расширения для получения количества столбцов матрицы
        public static int ColumnsCount(this double[,] matrix)
        {
            return matrix.GetUpperBound(1) + 1;
        }

        public static double[,] VectorToMatrix(this double[] vector)
        {
            var n = vector.GetLength(0);
            var res = new double[n, 1];

            for (int row = 0; row < n; row++)
            {
                res[row, 0] = vector[row];
            }

            return res;
        }

        public static double[] MatrixToVector(this double[,] matrix)
        {
            if (matrix.GetLength(1) != 1) throw new ArgumentException();

            var n = matrix.GetLength(0);

            var res = new double[n];

            for (int row = 0; row < n; row++)
            {
                res[row] = matrix[row, 0];
            }

            return res;
        }

        public static double[] SqueezeMean2(this double[] vector)
        {
            if (vector.Length % 2 != 0) throw new ArgumentException();
            var nn = vector.Length / 2;

            var res = new double[nn];

            for (int row = 0; row < nn; row++)
            {
                res[row] = (vector[2 * row] + vector[2 * row + 1]) / 2.0;
            }

            return res;
        }

        public static double[,] TransponseMatrix(this double[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new ArgumentException();

            var n = matrix.GetLength(0);
            var res = new double[n, n];

            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    res[row, col] = matrix[col, row];
                }
            }

            return res;
        }


        public static double[,] Normalize(this double[,] array)
        {
            double maxAbsValue = FindMaxAbsoluteValue(array);

            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);

            var res = new double[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    res[i, j] = array[i, j] / maxAbsValue;
                }
            }

            return res;
        }

        private static double FindMaxAbsoluteValue(double[,] array)
        {
            double maxAbsValue = 0.0;

            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    double absValue = Math.Abs(array[i, j]);
                    maxAbsValue = Math.Max(maxAbsValue, absValue);
                }
            }

            return maxAbsValue;
        }
    }
}