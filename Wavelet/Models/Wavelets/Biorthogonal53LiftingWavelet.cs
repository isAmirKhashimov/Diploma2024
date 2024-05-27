namespace Wavelet.Models.Wavelets
{
    internal class Biorthogonal53LiftingWavelet : LiftingWavelet
    {
        public override string Name => "Биортогональный вейвлет 5/3 (лифтинговая схема)";

        public Biorthogonal53LiftingWavelet(double[] xs, double[] ys)
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
            var nextLambdas = lambdas.Skip(1).Append(lambdas.Last()).ToArray();
            return ArrayUtils.DivideByElements(ArrayUtils.AddByElements(lambdas, nextLambdas), 2.0);
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
            var prevGammas = gammas.Take(1).Concat(gammas.Take(gammas.Length - 1)).ToArray();
            return ArrayUtils.AddByElements(ArrayUtils.DivideByElements(gammas, 4.0), ArrayUtils.DivideByElements(prevGammas, 4.0));
        }
    }
}
