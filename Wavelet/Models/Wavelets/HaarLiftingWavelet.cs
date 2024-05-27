namespace Wavelet.Models.Wavelets
{
    internal class HaarLiftingWavelet : LiftingWavelet
    {
        public override string Name => "Вейвлет Хаара (лифтинговая схема)";

        public HaarLiftingWavelet(double[] xs, double[] ys)
        {
            Xs = [.. xs];
            Cs = [.. ys];
            dds = [];

            while (Xs.Length > 1)
            {
                Squeeze();
            }

            Xs = [.. xs];
            Cs = [.. ys];
        }

        protected override double[] P(double[] lambdas)
        {
            return [.. lambdas];
        }

        protected override (double[] lambdas, double[] gammas) S(double[] xs)
        {
            var lambdas = xs.ToList().Where((_, index) => index % 2 == 0).ToArray();
            var gammas = xs.ToList().Where((_, index) => index % 2 != 0).ToArray();

            return (lambdas, gammas);
        }

        protected override double[] SBack(double[] lambdas, double[] gammas)
        {
            return lambdas.Zip(gammas).SelectMany(el => new[] { el.First, el.Second }).ToArray();
        }

        protected override double[] U(double[] gammas)
        {
            return ArrayUtils.DivideByElements(gammas, 2);
        }
    }
}
