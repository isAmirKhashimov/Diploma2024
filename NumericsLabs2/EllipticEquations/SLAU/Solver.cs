namespace SLAU
{
    internal static class Solver
    {
        public static double[] Solve(double[,] a, double[] b)
        {
            var A = new Matrix(a);
            var PLU = A.GetPLU();
            var B = new Row(b);
            var slau = Maths.SolveSlau(PLU, B);

            return slau.C;
        }

        public static double[] Solve3Diag(double[,] A, double[] d)
        {
            int n = d.Length;

            var abc = Maths.Decomposite(A);
            var pq = Maths.FindPQ(abc, d);
            var xn = (d[n - 1] - abc.a[n - 1] * pq.Q[n - 2]) / (abc.a[n - 1] * pq.P[n - 2] + abc.b[n - 1]);
            var x = Maths.FindXWithPQ(pq, xn);

            return x;
        }
    }
}
