using System;
using System.Linq;
using System.Text;

namespace SLAU
{
    public class Matrix
    {
        public Row[] V { get; set; }
        public int Rows { get => V.Length; }
        public int Columns { get => V[0].Length; }

        public Matrix(params Row[] v)
        {
            V = v.ToArray();
        }

        public Matrix(double[,] v)
        {
            var r = v.GetUpperBound(0) + 1;
            var c = v.GetUpperBound(1) + 1;
            V = new Row[r];
            for (int y = 0; y < r; y++)
            {
                V[y] = new Row(c);
                for (int x = 0; x < c; x++)
                {
                    this[y, x] = v[y, x];
                }
            }
        }

        public Matrix(int n): this(n, n) { }

        public Matrix(int r, int c)
        {
            V = new Row[r];
            for (int i = 0; i < r; i++)
            {
                V[i] = new Row(c);
            }
        }

        public static Matrix One(int n)
        {
            var result = new Matrix(n);
            for(int i = 0; i < n; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        public static Matrix AsColumn(Row r)
        {
            var result = new Matrix(r.Length, 1);
            for(int i = 0; i < r.Length; i++)
            {
                result[i, 0] = r[i];
            }
            return result;
        }

        public double this[int r, int c]
        {
            get => V[r][c];
            set => V[r][c] = value;
        }

        public Row this[int r]
        {
            get => V[r].Clone();
            set => Array.ConstrainedCopy(value.C, 0, V[r].C, 0, Columns);
        }

        public Row GetColumn(int c)
        {
            var result = new Row(Rows);
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = this[i, c];
            }
            return result;
        }

        public Matrix SetColumn(int c, Row cv)
        {
            var result = Clone();
            for (int i = 0; i < result.Rows; i++)
            {
                result[i,c] = cv[i];
            }
            return result;
        }

        public Matrix SwapRows(int r1, int r2)
        {
            var result = Clone();
            var r1value = result[r1];
            result[r1] = result[r2];
            result[r2] = r1value;
            return result;
        }

        public Matrix ForEach(Func<double, int, int, double> proc)
        {
            var result = Clone();
            for(int y = 0; y < Rows; y++)
            {
                for(int x = 0; x < Columns; x++)
                {
                    result[y, x] = proc(result[y, x], y, x);
                }
            }
            return result;
        }

        public (Matrix, Matrix, Matrix) GetPLU()
        {
            var n = V.Length;
            Matrix P = One(n);
            Matrix A = Clone();
            for (int i = 0; i < Rows; i++)
            {
                int maxi = i;
                for(int j = i + 1; j < Rows; j++)
                {
                    if(Math.Abs(A[j,i]) > Math.Abs(A[maxi,i]))
                    {
                        maxi = j;
                    }
                }
                A = A.SwapRows(i, maxi);
                P = P.SwapRows(i, maxi);
            }
            Matrix L = One(n);
            Matrix U = A.Clone();
            for (int k = 1; k <= n; k++)
            {
                Matrix Ul = U.Clone();
                for (int i = k + 1; i <= n; i++)
                {
                    double uk = Ul[i-1,k-1] / Ul[k-1,k-1];
                    L[i - 1,k-1] = uk;
                    for (int j = k; j <= n; j++)
                    {
                        U[i-1,j-1] = Ul[i-1,j-1] - uk * Ul[k-1,j-1];
                    }
                }
            }
            return (P, L, U);
        }

        public double GetTriangleDeterminant()
        {
            if(Rows != Columns)
            {
                throw new Exception("Not square matrix");
            }
            double result = 1.0;
            for(int i = 0; i < Rows; i++)
            {
                result *= this[i, i];
            }
            return Math.Round(result * 10000) / 10000;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            var result = new Matrix(a.Rows, b.Columns);
            for (int y = 0; y < result.Rows; y++)
            {
                for (int x = 0; x < result.Columns; x++)
                {
                    for (int i = 0; i < a.Columns; i++)
                    {
                        result[y, x] += a[y, i] * b[i, x];
                    }
                }
            }
            return result;
        }

        public static Matrix operator *(double a, Matrix b)
        {
            var result = b.Clone();
            for (int y = 0; y < result.Rows; y++)
            {
                for (int x = 0; x < result.Columns; x++)
                {
                    result[y, x] *= a;
                }
            }
            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            var result = a.Clone();
            for (int y = 0; y < result.Rows; y++)
            {
                for (int x = 0; x < result.Columns; x++)
                {
                    result[y, x] -= b[y, x];
                }
            }
            return result;
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if(a.Rows != b.Rows || a.Columns != b.Columns)
            {
                return false;
            }
            for(int y = 0; y < a.Rows; y++)
            {
                for(int x = 0; x < a.Columns; x++)
                {
                    if(a[y, x] != b[y, x])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }


        public Matrix Clone()
        {
            var columns = new Row[V.Length];
            for(int i = 0; i < V.Length; i++)
            {
                columns[i] = new Row(V[i].Length);
                Array.ConstrainedCopy(V[i].C, 0, columns[i].C, 0, V[i].Length);
            }
            return new Matrix(columns);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var i in V)
            {
                sb.Append(i.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}
