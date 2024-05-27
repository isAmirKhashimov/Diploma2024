using System.Windows;
using Wavelet.Models;
using Wavelet.Models.Wavelets;

namespace Wavelet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double[] xs;
        private double[] ys;
        private Func<double, double> f = (x) => 1 * Math.Sin(550 * x) + 10 * x * Math.Sin(x);
        private List<IWavelet> wavelets;
        private IWavelet wavelet;

        /*readonly double[] g = [
            -1.0 / 8.0,
            1.0 / 4.0,
            3.0 / 4.0,
            1.0 / 4.0,
            -1.0 / 8.0
        ];

        readonly double[] h = [
            -1.0 / 2.0,
            1.0,
            -1.0 / 2.0
        ];*/

        public MainWindow()
        {
            xs = ArrayUtils.Linspace(-5, 5, 256);
            ys = xs.Select(f).ToArray();
            wavelets =
            [
                new HaarWavelet(xs, ys),
                new DobeshiWavelet(xs, ys),
                new HaarLiftingWavelet(xs, ys),
                new Biorthogonal53LiftingWavelet(xs, ys),
            ];

            InitializeComponent();

            WaveletComboBox.ItemsSource = wavelets;
            WaveletComboBox.DisplayMemberPath = "Name";
            WaveletComboBox.SelectedIndex = 0;

            RefreshFunctions();
        }

        private void RefreshFunctions()
        {
            OriginalFunc.Plot.Clear();
            OriginalFunc.Plot.AddScatter(xs, ys);
            OriginalFunc.Plot.YLabel("y");
            OriginalFunc.Refresh();
            
            Scalegramm.Plot.Clear();
            var hm = Scalegramm.Plot.AddHeatmap(wavelet.Scalegramm, ScottPlot.Drawing.Colormap.Solar);
            hm.Smooth = true;
            var cb = Scalegramm.Plot.AddColorbar(hm);
            Scalegramm.Plot.XLabel("x");
            Scalegramm.Plot.YLabel("y");
            Scalegramm.Refresh();
            
            CFunc.Plot.Clear();
            CFunc.Plot.AddScatter(wavelet.Xs, wavelet.Cs);
            CFunc.Plot.YLabel("y");
            CFunc.Refresh();


            DFunc.Plot.Clear();

            if (wavelet.Ds is not null)
            {
                DFunc.Plot.AddScatter(wavelet.Xs, wavelet.Ds);
            }

            DFunc.Plot.YLabel("y");
            DFunc.Refresh();
        }



        private void Squeeze_ButtonClick(object sender, RoutedEventArgs e)
        {
            _ = wavelet.Squeeze();
            RefreshFunctions();
        }

        private void ExtendSilently_ButtonClick(object sender, RoutedEventArgs e)
        {
            _ = wavelet.ExtendWithoutNoise();
            RefreshFunctions();
        }
        private void ExtendNoisy_ButtonClick(object sender, RoutedEventArgs e)
        {
            _ = wavelet.ExtendWithNoise();
            RefreshFunctions();
        }

        private void Wavelet_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            wavelet = (IWavelet)WaveletComboBox.SelectedItem;
            RefreshFunctions();
        }
    }
}