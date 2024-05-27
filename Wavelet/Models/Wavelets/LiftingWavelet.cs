namespace Wavelet.Models.Wavelets
{
    internal abstract class LiftingWavelet : IWavelet
    {
        public Dictionary<int, double[]> dds;

        public double[] Cs { get; set; }

        public double[]? Ds => dds.TryGetValue(Xs.Length, out var ds) ? ds : null;

        public double[] Xs { get; set; }

        public abstract string Name { get; }

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
            var (lambdas, gammas) = S(Cs);
            gammas = ArrayUtils.SubtractByElements(gammas, P(lambdas));
            lambdas = ArrayUtils.AddByElements(lambdas, U(gammas));

            Cs = lambdas;
            Xs = ArrayUtils.Reshape(Xs, Cs.Length);
            
            dds.TryAdd(Xs.Length, gammas);

            return (Cs!, Ds!);
        }

        public double[] ExtendWithNoise()
        {
            var lambdas = Cs!;
            var gammas = Ds ?? ArrayUtils.ZerosLike(Cs);
            lambdas = ArrayUtils.SubtractByElements(lambdas, U(gammas));
            gammas = ArrayUtils.AddByElements(gammas, P(lambdas));

            Cs = SBack(lambdas, gammas);
            Xs = ArrayUtils.Reshape(Xs, Cs.Length);
            return Cs;
        }

        public double[] ExtendWithoutNoise()
        {
            var lambdas = Cs!;
            var gammas = ArrayUtils.ZerosLike(Cs);
            lambdas = ArrayUtils.SubtractByElements(lambdas, U(gammas));
            gammas = ArrayUtils.AddByElements(gammas, P(lambdas));

            Cs = SBack(lambdas, gammas);
            Xs = ArrayUtils.Reshape(Xs, Cs.Length);
            return Cs;
        }

        protected abstract (double[] lambdas, double[] gammas) S(double[] X);
        protected abstract double[] SBack(double[] lambdas, double[] gammas);
        protected abstract double[] P(double[] lambdas);
        //protected abstract double[] PBack(double[] lambdas);
        protected abstract double[] U(double[] gammas);
        //protected abstract double[] UBack(double[] gammas);
    }
}
