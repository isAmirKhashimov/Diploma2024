namespace Wavelet.Models.Wavelets
{
    internal abstract class OrthogonalWavelet2D
    {/*
        public double[,] xs;
        public double[,] cs;
        public Dictionary<int, (double[,] D1, double[,] D2, double[,] D3)> dds;

        public double[,] Cs => cs;

        public double[,]? Ds1 => dds.TryGetValue(xs.Length, out var ds) ? ds.D1 : null;
        public double[,]? Ds2 => dds.TryGetValue(xs.Length, out var ds) ? ds.D2 : null;
        public double[,]? Ds3 => dds.TryGetValue(xs.Length, out var ds) ? ds.D3 : null;

        public double[,] Xs => xs;

        public abstract string Name { get; }

        protected abstract double[] G { get; }

        protected abstract double[] H { get; }

        public (double[,] C, double[,] D1, double[,] D2, double[,] D3) Squeeze()
        {
            var (AH, AG) = GenerateA(H, G, xs.Length);
            var AHX = ArrayUtils.Multiply(AH, Cs);
            var AGX = ArrayUtils.Multiply(AG, Cs);
            var n = xs.Length / 2;

            xs = ArrayUtils.Reshape(xs, n, n);
            cs = ArrayUtils.Multiply(AHX, Cs);
            cs = ArrayUtils.Multiply(AGX, Cs);
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


        private static (double[,] AH, double[,] AG) GenerateA(double[] h, double[] g, int n)
        {
            if (n % 2 != 0) throw new ArgumentException();

            var period = 2;
            var nn = n / 2;
            var resH = new double[nn, n];
            var resG = new double[nn, n];

            for (int rowg = 0; rowg < nn; rowg++)
            {
                var st = rowg * period;

                for (int col = st, gidx = 0; gidx < h.Length; gidx++, col++)
                {
                    if (col >= n)
                    {
                        col = 0;
                    }

                    resH[rowg, col] = h[gidx];
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

                    resG[rowh, col] = g[hidx];
                }
            }

            return (resH, resG);
        }

        private double[] ExtendWith(double[] ds)
        {
            var n = xs.Length * 2;
            var A = GenerateA(H, G, n).TransponseMatrix();
            cs = ArrayUtils.Multiply(A, [.. cs, .. ds]);

            xs = ArrayUtils.Reshape(xs, n);
            return cs;
        }*/
    }
}
