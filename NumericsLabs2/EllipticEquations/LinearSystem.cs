using System;
using System.Collections.Generic;
using System.Linq;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using Column = System.Collections.Generic.List<double>;
using Solution = System.Collections.Generic.List<double>;

namespace Numerics7
{
    internal class LinearSystem
    {
        private Matrix A;
        private Column D;

        public LinearSystem(List<Equation> equations)
        {
            A = new Matrix(equations.Select(eq => eq.Coefs.ToList()));
            D = new Column(equations.Select(eq => eq.Result));
        }

        public Solution Solve()
        {
            var (P, L, U) = PLU(ListToArray2D(A));
            var pb = Multiply(P, D.ToArray());
            var z = SolveL(L, pb);
            var x = SolveU(U, z);
            return x.ToList();
        }

        public Solution Solve3Dim()
        {
            int n = D.Count;

            Make3Diag();
            var abc = Decomposite(ListToArray2D(A));
            var pq = FindPQ(abc, D.ToArray());
            var xn = (D[n - 1] - abc.a[n - 1] * pq.Q[n - 2]) / (abc.a[n - 1] * pq.P[n - 2] + abc.b[n - 1]);
            var x = FindXWithPQ(pq, xn);

            return x.ToList();
        }

        private void Make3Diag()
        {
            int n = D.Count;
            if (A[0][2] != 0)
            {
                var koef = -A[0][2] / A[1][2];
                
                for (var i = 0; i < 3; i++)
                {
                    A[0][i] += koef * A[1][i];
                }
 
                D[0] += koef * D[1];
            }

            if (A[n - 1][n - 3] != 0)
            {
                var koef = -A[n - 1][n - 3] / A[n - 2][n - 3];

                for (var i = 1; i <= 3; i++)
                {
                    A[n - 1][n - i] += koef * A[n - 2][n - i];
                }

                D[n - 1] += koef * D[n - 2];
            }
        }

        private T[,] ListToArray2D<T>(List<List<T>> lists)
        {
            T[,] arrays = new T[lists.Count, lists[0].Count];
            for (int i = 0; i < lists.Count; i++)
            {
                for (int j = 0; j < lists[i].Count; j++)
                {
                    arrays[i, j] = lists[i][j];
                }

            }
            return arrays;
        }

        private double[] SolveL(double[,] L, double[] b)
        {
            int rows = b.Length;
            double[] z = new double[rows];

            z[0] = b[0];
            for (int i = 1; i < rows; i++)
            {
                z[i] = b[i];
                for (int j = 0; j < i; j++)
                {
                    z[i] -= L[i, j] * z[j];
                }
            }
            return z;
        }

        private double[] SolveU(double[,] U, double[] z)
        {
            int rows = z.Length;
            double[] x = new double[rows];

            x[rows - 1] = z[rows - 1] / U[rows - 1, rows - 1];
            for (int i = rows - 2; i >= 0; i--)
            {
                x[i] = z[i];
                for (int j = rows - 1; j > i; j--)
                {
                    x[i] -= U[i, j] * x[j];
                }
                x[i] /= U[i, i];
            }
            return x;
        }

        private double[,] Multiply(double[,] matrixA, double[,] matrixB)
        {
            if (matrixA.GetLength(1) != matrixB.GetLength(0))
            {
                throw new ArgumentException();
            }
            int rows = matrixA.GetLength(0);
            int cols = matrixB.GetLength(1);
            int iters = matrixA.GetLength(1);
            var res = new double[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    res[row, col] = 0;
                    for (int iter = 0; iter < iters; iter++)
                    {
                        res[row, col] += matrixA[row, iter] * matrixB[iter, col];
                    }
                }
            }
            return res;
        }

        private double[] Multiply(double[,] matrixA, double[] vectorB)
        {
            return Multiply(matrixA, VectorToMatrix(vectorB)).Cast<double>().ToArray();
        }

        private (double[,], double[,], double[,]) PLU(double[,] matrixA)
        {
            int n = matrixA.GetLength(0);
            if (n != matrixA.GetLength(1))
            {
                throw new ArgumentException();
            }

            var L = new double[n, n];
            var U = new double[n, n];
            var P = new int[n];
            for (int i = 0; i < n; i++)
            {
                P[i] = i;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    L[i, j] = i == j ? 1 : 0;
                    U[i, j] = matrixA[i, j];
                }
            }

            for (int k = 0; k < n; k++)
            {
                var maxx = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(U[i, k]) > Math.Abs(U[maxx, k]))
                    {
                        maxx = i;
                    }
                }
             
                Swap(ref P[maxx], ref P[k]);
                for (int i = 0; i < n; i++)
                {
                    Swap(ref U[k, i], ref U[maxx, i]);
                }
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    L[i, k] = U[i, k] / U[k, k];
                    for (int j = k; j < n; j++)
                    {
                        U[i, j] = U[i, j] - L[i, k] * U[k, j];
                    }
                }

            }

            var Pres = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Pres[i, j] = P[i] == j ? 1 : 0;
                }
            }

            return (Pres, L, U);
        }

        private T[,] VectorToMatrix<T>(T[] vector)
        {
            var res = new T[vector.Length, 1];
            for (int i = 0; i < vector.Length; i++)
            {
                res[i, 0] = vector[i];
            }
            return res;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }

        private static (double[] P, double[] Q) FindPQ((double[] a, double[] b, double[] c) abc, double[] d)
        {
            int n = abc.a.Length;
            var P = new double[n];
            var Q = new double[n];
            P[0] = -abc.c[0] / abc.b[0];
            Q[0] = d[0] / abc.b[0];
            for (int i = 1; i < n - 1; i++)
            {
                P[i] = -abc.c[i] / (abc.a[i] * P[i - 1] + abc.b[i]);
                Q[i] = (d[i] - abc.a[i] * Q[i - 1]) / (abc.a[i] * P[i - 1] + abc.b[i]);
            }

            return (P, Q);
        }

        private static double[] FindXWithPQ((double[] P, double[] Q) pq, double xn)
        {
            var n = pq.P.Length;
            var x = new double[n];

            x[n - 1] = xn;
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = pq.P[i] * x[i + 1] + pq.Q[i];
            }
            return x;
        }

        private static (double[] a, double[] b, double[] c) Decomposite(double[,] A)
        {
            var n = A.GetLength(0);
            var a = new double[n];
            var b = new double[n];
            var c = new double[n];
            b[0] = A[0, 0];
            c[0] = A[0, 1];
            a[n - 1] = A[n - 1, n - 2];
            b[n - 1] = A[n - 1, n - 1];

            for (int i = 1; i < n - 1; i++)
            {
                a[i] = A[i, i - 1];
                b[i] = A[i, i];
                c[i] = A[i, i + 1];
            }
            return (a, b, c);
        }

    }
}
