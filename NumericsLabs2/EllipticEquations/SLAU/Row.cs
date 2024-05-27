using System;
using System.Text;

namespace SLAU
{
    public class Row
    {
        public double[] C { get; set; }
        public int Length { get => C.Length; }

        public Row(int n, double val = 0)
        {
            C = new double[n];
            for (int i = 0; i < n; i++)
            {
                C[i] = val;
            }
        }

        public Row(double[] c)
        {
            C = new double[c.Length];
            Array.ConstrainedCopy(c, 0, C, 0, c.Length);
        }

        public Row Cut(int f, int t)
        {
            var result = new Row(t - f + 1);
            Array.ConstrainedCopy(C, f, result.C, 0, result.Length);
            return result;
        }

        public Row Cut(int f)
        {
            return Cut(f, C.Length - 1);
        }

        public Matrix AsColumn()
        {
            var result = new Matrix(Length,1);
            for(int i = 0; i < Length; i++)
            {
                result[i, 0] = C[i];
            }
            return result;
        }

        public double this[int i]
        {
            get => C[i];
            set => C[i] = value;
        }

        public int CountFirstZero()
        {
            for(int i = 0; i < Length; i++)
            {
                if(C[i] != 0)
                {
                    return i;
                }
            }
            return Length;
        }

        public double Norm()
        {
            double ans = 0;
            foreach(var n in C)
            {
                ans += n * n;
            }
            return Math.Sqrt(ans);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var j in C)
            {
                var s = string.Format("{0:0.0;-0.0}", j);
                if (s.Length > 5)
                {
                    s = s.Substring(0, 5);
                }
                s = new string(' ', 5 - s.Length) + s;
                sb.Append(s).Append(' ');
            }
            return sb.ToString();
        }

        public static bool operator ==(Row a, Row b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int x = 0; x < a.Length; x++)
            {
                if(a[x] != b[x])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Row a, Row b)
        {
            return !(a == b);
        }

        public Row Clone()
        {
            var result = new Row(Length);
            Array.ConstrainedCopy(C, 0, result.C, 0, Length);
            return result;
        }
    }
}
