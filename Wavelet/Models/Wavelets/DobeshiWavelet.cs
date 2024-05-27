namespace Wavelet.Models.Wavelets
{
    internal class DobeshiWavelet : OrthogonalWavelet
    {
        protected readonly double[] g = [
            -(1 - Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (3 - Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            -(3 + Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (1 + Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
        ];

        protected readonly double[] h = [
            (1 + Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (3 + Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (3 - Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (1 - Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
        ];

        public DobeshiWavelet(double[] xs, double[] ys)
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

        public override string Name => "Вейвлет Добеши";

        protected override double[] G => g;

        protected override double[] H => h;
    }
}
