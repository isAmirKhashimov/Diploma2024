using System.Net.WebSockets;

namespace Wavelet.Models.Wavelets
{
    internal abstract class OrthogonalWavelet : IWavelet
    {
        public double[] xs;
        public double[] cs;
        public Dictionary<int, double[]> dds;

        public double[] Cs => cs;

        public double[]? Ds => dds.TryGetValue(xs.Length, out var ds) ? ds : null;

        public double[] Xs => xs;

        public abstract string Name { get; }

        protected abstract double[] G { get; }

        protected abstract double[] H { get; }

        public double[,]? Scalegramm
        {
            get
            {
                /*
                 * rows = log2 Key + 1
                 * cols = sorted.Last().Count;
                 * .
                 * 
                 */
                var sortedDs = dds.OrderBy(dskvp => dskvp.Key).ToList();
                var last = sortedDs.Last();
                var rows = 1;
                var birow = 1;

                while (birow < last.Key)
                {
                    rows++;
                    birow <<= 1;
                }

                var cols = last.Value.Length;

                var rrrows = rows * 5;
                var res = new double[rrrows, cols];
                birow = 1;

                for (int row = 0; row < rrrows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        res[row, col] = sortedDs[row / (rrrows / rows)].Value[col % (birow)];
                    }

                    if ((row + 1) % (rrrows / rows) == 0)
                    {
                        birow <<= 1;
                    }
                }

                return res.Normalize();
            }
        }

        public (double[] C, double[] D) Squeeze()
        {
            var A = GenerateA(H, G, xs.Length);
            var ass = ArrayUtils.Multiply(A, cs);
            var n = xs.Length / 2;

            xs = ArrayUtils.Reshape(xs, n);
            cs = ass.Take(n).ToArray();
            dds.TryAdd(n, ass.TakeLast(n).ToArray());

            return (cs, dds[n]);
        }

        public double[] ExtendWithNoise()
        {
            if (dds.TryGetValue(xs.Length, out var ds))
            {
                return ExtendWith(ds);
            }
            else
            {
                return ExtendWith(ArrayUtils.ZerosLike(cs));
            }
        }

        public double[] ExtendWithoutNoise() => ExtendWith(ArrayUtils.ZerosLike(cs));


        private static double[,] GenerateA(double[] h, double[] g, int n)
        {
            if (n % 2 != 0) throw new ArgumentException();

            var period = 2;
            var nn = n / 2;
            var res = new double[n, n];

            for (int rowg = 0; rowg < nn; rowg++)
            {
                var st = rowg * period;

                for (int col = st, gidx = 0; gidx < h.Length; gidx++, col++)
                {
                    if (col >= n)
                    {
                        col = 0;
                    }

                    res[rowg, col] = h[gidx];
                }
            }

            for (int rowh = nn; rowh < n; rowh++)
            {
                var st = (rowh - nn) * period;

                for (int col = st, hidx = 0; hidx < g.Length; hidx++, col++)
                {
                    if (col >= n)
                    {
                        col = 0;
                    }

                    res[rowh, col] = g[hidx];
                }
            }

            return res;
        }

        private double[] ExtendWith(double[] ds)
        {
            var n = xs.Length * 2;
            var A = GenerateA(H, G, n).TransponseMatrix();
            cs = ArrayUtils.Multiply(A, [.. cs, .. ds]);

            xs = ArrayUtils.Reshape(xs, n);
            return cs;
        }
    }
}
