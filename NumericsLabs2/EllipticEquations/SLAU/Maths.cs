using System;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;

namespace SLAU
{
    public class Maths
    {
        public static Row SolveTriangle(Matrix x, Row b)
        {
            var result = new Row(b.Length, double.PositiveInfinity);
            for (int k = 0; k < b.Length; k++)
            {
                for (int i = 0; i < b.Length; i++)
                {
                    if(result[i] != double.PositiveInfinity)
                    {
                        continue;
                    }
                    var nf = 0;
                    var nfi = -1;
                    for(int j=0;j<b.Length;j++)
                    {
                        if(x[i][j] != 0 && result[j] == double.PositiveInfinity)
                        {
                            nf++;
                            nfi = j;
                        }
                    }
                    if(nf == 1)
                    {
                        double sum = 0;
                        for (int j = 0; j < b.Length; j++)
                        {
                            if (result[j] != double.PositiveInfinity)
                            {
                                sum += result[j] * x[i, j];
                            }
                        }
                        result[i] = (b[i] - sum) / x[i, nfi];
                        break;
                    }
                }
            }
            return result;
        }

        public static Row SolveProgonka(Matrix A, Row B)
        {
            int n = A.Rows;
            double[] a = new double[n];
            double[] b = new double[n];
            double[] c = new double[n];
            double[] d = new double[n];
            double[] p = new double[n];
            double[] q = new double[n];
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = i == 0 ? 0 : A[i, i - 1];
                b[i] = A[i, i];
                c[i] = i == n - 1 ? 0 : A[i, i + 1];
                d[i] = B[i];
            }
            p[0] = -c[0] / b[0];
            q[0] = d[0] / b[0];
            for(int i = 1; i < n; i++)
            {
                p[i] = -c[i] / (b[i] + a[i] * p[i - 1]);
                q[i] = (d[i] - a[i] * q[i - 1]) / (b[i] + a[i] * p[i - 1]);
            }
            x[n - 1] = q[n - 1];
            for(int i = n - 2; i >= 0; i--)
            {
                x[i] = p[i] * x[i + 1] + q[i];
            }
            return new Row(x);
        }

        public static Row SolveSlau((Matrix p, Matrix l, Matrix u) plu, Row b)
        {
            var l = SolveTriangle(plu.l, (plu.p * Matrix.AsColumn(b)).GetColumn(0));
            return SolveTriangle(plu.u, l);
        }

        public static (Row result, int iters) SolveIterations(Matrix c, Row d, double e, bool zeidel)
        {
            var n = c.Rows;
            var x = new Row(n);
            var a = new Matrix(n, n);
            var b = new Row(n);
            for(int i = 0; i < n; i++)
            {
                if(c[i,i] == 0)
                {
                    throw new Exception("Zero diagonal element");
                }
                for(int j = 0; j < n; j++)
                {
                    if(j != i)
                    {
                        a[i, j] = -c[i, j] / c[i, i];
                    }
                    else
                    {
                        a[i, j] = 0;
                    }
                }
                b[i] = d[i] / c[i, i];
            }
            double g = 0;
            for(int i = 0; i < n; i++)
            {
                double h = 0;
                for(int j = 0; j < n; j++)
                {
                    if(a[i,j] > 0)
                    {
                        h = h + a[i, j];
                    }
                    else
                    {
                        h = h - a[i, j];
                    }
                }
                if(h > g)
                {
                    g = h;
                }
            }
            if(g >= 1)
            {
                throw new Exception("Sxodimost condition not satisfied");
            }
            double f = 0;
            for(int i = 0; i < n; i++)
            {
                x[i] = b[i];
                if(x[i] > f)
                {
                    f = x[i];
                }
                if(-x[i] > f)
                {
                    f = -x[i];
                }
            }
            f = f * g / (1 - g);
            int k = 0;
            while(f > e)
            {
                var y = new Row(n);
                for(int i = 0; i < n; i++)
                {
                    y[i] = x[i];
                }
                for(int i = 0; i < n; i++)
                {
                    x[i] = b[i];
                    if(zeidel)
                    {
                        // Zeidel method
                        for (int j = 0; j < i; j++)
                        {
                            x[i] = x[i] + a[i, j] * x[j];
                        }
                        for (int j = i; j < n; j++)
                        {
                            x[i] = x[i] + a[i, j] * y[j];
                        }
                    }
                    else
                    {
                        // Iteration method
                        for (int j = 0; j < n; j++)
                        {
                            x[i] = x[i] + a[i, j] * y[j];
                        }
                    }
                }
                f = 0;
                for(int i = 0; i < n; i++)
                {
                    if(x[i] - y[i] > f)
                    {
                        f = x[i] - y[i];
                    }
                    if (y[i] - x[i] > f)
                    {
                        f = y[i] - x[i];
                    }
                }
                f = f * g / (1 - g);
                k = k + 1;
            }
            return (x, k - 1);
        }

        public static (Row sobsZnaj, Matrix sobsVect, int iters) SolveYakobiRotation(Matrix a, double e)
        {
            a = a.Clone();
            int n = a.Rows;
            Matrix v = new Matrix(n, n);
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if(i == j)
                    {
                        v[i, j] = 1;
                    }
                    else
                    {
                        v[i, j] = 0;
                    }
                }
            }
            double f = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    f = f + a[i, j] * a[i, j];
                }
            }
            f = Math.Sqrt(f);
            int k = 0;
            while(f > e)
            {
                double g = 0;
                int l = 1;
                int m = 2;
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        if(a[i,j] > g)
                        {
                            g = a[i, j];
                            l = i;
                            m = j;
                        }
                        if(-a[i,j] > g)
                        {
                            g = -a[i, j];
                            l = i;
                            m = j;
                        }
                    }
                }
                Matrix u = new Matrix(n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if(i == j)
                        {
                            u[i, j] = 1;
                        }
                        else
                        {
                            u[i, j] = 0;
                        }
                    }
                }
                double h;
                if(a[l,l] == a[m,m])
                {
                    h = Math.PI / 4;
                }
                else
                {
                    h = Math.Atan(2 * a[l, m] / (a[l, l] - a[m, m])) / 2;
                }
                u[l, l] = Math.Cos(h);
                u[l, m] = -Math.Sin(h);
                u[m, l] = Math.Sin(h);
                u[m, m] = Math.Cos(h);
                Matrix b = new Matrix(n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if(i == l || i == m || j == l || j == m)
                        {
                            b[i, j] = 0;
                            for(int p = 0; p < n; p++)
                            {
                                b[i, j] = b[i, j] + u[p, i] * a[p, j];
                            }
                        }
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == l || i == m || j == l || j == m)
                        {
                            a[i, j] = 0;
                            for (int p = 0; p < n; p++)
                            {
                                a[i, j] = a[i, j] + b[i, p] * u[p, j];
                            }
                        }
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == l || i == m || j == l || j == m)
                        {
                            b[i, j] = 0;
                            for (int p = 0; p < n; p++)
                            {
                                b[i, j] = b[i, j] + v[i, p] * u[p, j];
                            }
                        }
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == l || i == m || j == l || j == m)
                        {
                            v[i, j] = b[i, j];
                        }
                    }
                }
                f = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        f = f + a[i, j] * a[i, j];
                    }
                }
                f = Math.Sqrt(f);
                k = k + 1;
            }
            Row w = new Row(n);
            for(int i = 0; i < n; i++)
            {
                w[i] = a[i, i];
            }
            return (w, v, k);
        }

        public static (double[] P, double[] Q) FindPQ((double[] a, double[] b, double[] c) abc, double[] d)
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

        public static double[] FindXWithPQ((double[] P, double[] Q) pq, double xn)
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

        public static (double[] a, double[] b, double[] c) Decomposite(double[,] A)
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


        public static Complex[] SolveQr(Matrix a, double e)
        {
            int n = a.Rows;
            var A_i = a.Clone();
            var eigen_values = new List<Complex>();
            var i = 0;
            while (i < n)
            {
                (Complex[] cur_eigen_values, Matrix A_i_plus_1) = get_eigen_value(A_i, i, e);
                // complex or real
                eigen_values.AddRange(cur_eigen_values);
                i += cur_eigen_values.Length;
                A_i = A_i_plus_1;
            }
            return eigen_values.ToArray();
        }

        public static Matrix GetInverseMatrix((Matrix p,Matrix l,Matrix u) plu)
        {
            var result = new Matrix(plu.u.Rows, plu.u.Columns);
            for(int c = 0; c < result.Columns; c++)
            {
                var col = SolveSlau(plu, Matrix.One(plu.u.Rows).GetColumn(c));
                result = result.SetColumn(c, col);
            }
            return result;
        }

        private static (Complex[], Matrix) get_eigen_value(Matrix A, int i, double eps)
        {
            var A_i = A.Clone();
            while (true)
            {
                (Matrix Q, Matrix R) = QR_decomposition(A_i);
                A_i = R * Q;
                if(A_i.GetColumn(i + 1).Cut(i).Norm() <= eps)
                    return (new Complex[] { A_i[i][i] }, A_i);
                else if(A_i.GetColumn(i + 2).Cut(i).Norm() <= eps && is_complex(A_i, i, eps))
                    return (get_roots(A_i, i), A_i);
            }
        }

        private static bool is_complex(Matrix A, int i, double eps)
        {
            (Matrix Q, Matrix R) = QR_decomposition(A);
            var A_next = R * Q;
            var lambda1 = get_roots(A, i);
            var lambda2 = get_roots(A_next, i);
            return Complex.Abs(lambda1[0] - lambda2[0]) <= eps && Complex.Abs(lambda1[1] - lambda2[1]) <= eps;
        }

        private static Complex[] get_roots(Matrix A, int i)
        {
            int n = A.Rows;
            var a11 = A[i][i];
            var a12 = i + 1 < n ? A[i][i + 1] : 0;
            var a21 = i + 1 < n ? A[i + 1][i] : 0;
            var a22 = i + 1 < n ? A[i + 1][i + 1] : 0;
            return FindRoots.Polynomial(new double[] { 1, -a11 - a22, a11 * a22 - a12 * a21 });
        }

        private static (Matrix, Matrix) QR_decomposition(Matrix A)
        {
            int n = A.Rows;
            var Q = Matrix.One(n);
            var A_i = A.Clone();
            for (int i = 0; i < n; i++)
            {
                var H = get_householder_matrix(A_i, i);
                Q = Q * H;
                A_i = H * A_i;
            }
            return (Q, A_i);
        }

        private static Matrix get_householder_matrix(Matrix A, int col_num)
        {
            int n = A.Rows;
            var v = new Row(n);
            var a = A.GetColumn(col_num);
            v[col_num] = a[col_num] + Math.Sign(a[col_num]) * a.Cut(col_num).Norm();
            for (int i = col_num + 2; i < n; i++)
                v[i] = a[i];
            var H = Matrix.One(n) - (2 / (v.AsColumn() * new Matrix(v)).GetColumn(0)[0]) *(new Matrix(v) * v.AsColumn());
            return H;
        }


    }
}
