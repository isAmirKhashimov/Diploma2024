using ScottPlot;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;


namespace NumericsLabs3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller controller;

        public MainWindow()
        {
            InitializeComponent();

            controller = new Controller();

            CalcComboBox.ItemsSource = controller.CalculationNames;
            CalcComboBox.SelectedIndex = 0;

            controller.SetNthIssue(3);
            RefreshIssue();

            T_Slider.Minimum = 0;
            T_Slider.Maximum = controller.Issue.Parameters.K;
            T_Slider.TickFrequency = 1;
            T_Slider.Value = 0;
            T_Slider.IsSnapToTickEnabled = true;

            Y_Slider.Minimum = 0;
            Y_Slider.Maximum = controller.Issue.Parameters.Ny;
            Y_Slider.TickFrequency = 1;
            Y_Slider.Value = 0;
            Y_Slider.IsSnapToTickEnabled = true;

            RefreshGraph();
            RefreshIterGraph();
            RefreshAnalyticalGraph();
            RefreshErrorGraph();
        }

        private void RefreshIssue()
        {
            controller.Refresh();
        }

        private void RefreshGraph()
        {
            GraphVisualizer.Plot.Clear();

            int t = (int)(T_Slider.Value);
            int y = (int)(Y_Slider.Value);
            GraphVisualizer.Plot.AddScatter(controller.RangeOfX, controller.GetResultByTandYIndex(t, y), System.Drawing.Color.Red, 2, 2);
            GraphVisualizer.Plot.AddScatter(controller.RangeOfX, controller.GetAnalyticalResultByTandYIndex(t, y), System.Drawing.Color.Blue, 3, 3);
            GraphVisualizer.Plot.XLabel("x");
            GraphVisualizer.Plot.YLabel($"u(x, y={y * controller.Issue.Parameters.Hy}, k = {t * controller.Issue.Parameters.Tau})");

            GraphVisualizer.Refresh();
        }

        private void RefreshIterGraph()
        {
            int t = (int)(T_Slider.Value);
            HeatVisualizer.Plot.Clear();
            var hm = HeatVisualizer.Plot.AddHeatmap(controller.GetResultByTIndex(t), ScottPlot.Drawing.Colormap.Balance);
            hm.Smooth = true;
            var cb = HeatVisualizer.Plot.AddColorbar(hm);
            HeatVisualizer.Plot.XLabel("x");
            HeatVisualizer.Plot.YLabel("y");
            HeatVisualizer.Refresh();
        }

        private void RefreshAnalyticalGraph()
        {
            int t = (int)(T_Slider.Value);
            AnanylitcalHeatVisualizer.Plot.Clear();
            var hm = AnanylitcalHeatVisualizer.Plot.AddHeatmap(controller.GetAnalyticalResultByTIndex(t), ScottPlot.Drawing.Colormap.Balance);
            hm.Smooth = true;
            var cb = AnanylitcalHeatVisualizer.Plot.AddColorbar(hm);
            AnanylitcalHeatVisualizer.Plot.XLabel("x");
            AnanylitcalHeatVisualizer.Plot.YLabel("y");
            AnanylitcalHeatVisualizer.Refresh();
        }

        private void RefreshErrorGraph()
        {
            ErrorVisualizer.Plot.Clear();
            ErrorVisualizer.Plot.AddScatter(controller.RangeOfT, controller.GetError());
            ErrorVisualizer.Plot.XLabel("t");
            ErrorVisualizer.Plot.YLabel("ε(t)");
            ErrorVisualizer.Refresh();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshGraph();
        }

        private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (controller?.Issue != null)
            {
                controller.SetCalculator(CalcComboBox.SelectedItem.ToString());
                RefreshIssue();
                RefreshGraph();
                RefreshIterGraph();
                RefreshErrorGraph();
            }
        }

        private void Iteration_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshIterGraph();
            RefreshGraph();
            RefreshAnalyticalGraph();
        }
    }
}
