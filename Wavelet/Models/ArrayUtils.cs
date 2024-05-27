namespace Wavelet.Models
{
    internal static class ArrayUtils
    {
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
