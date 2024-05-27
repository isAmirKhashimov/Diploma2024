namespace Wavelet.Models.Wavelets
{
    internal class HaarWavelet : OrthogonalWavelet
    {
        protected readonly double[] g = [
            (1.0 / Math.Sqrt(2)),
            (-1.0 / Math.Sqrt(2)),
        ];

        protected readonly double[] h = [
            (1.0 / Math.Sqrt(2)),
            (1.0 / Math.Sqrt(2)),
        ];
        public override string Name => "Вейвлет Хаара";

        public HaarWavelet(double[] xs, double[] ys)
        {
            this.xs = [.. xs];
            cs = [.. ys];
            dds = [];
            
            while (this.xs.Length > 1)
            {
                Squeeze();
            }

            this.xs = [.. xs];
            cs = [.. ys];

        }

        protected override double[] G => g;

        protected override double[] H => h;
    }
}
