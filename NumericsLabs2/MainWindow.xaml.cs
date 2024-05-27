using ScottPlot;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace NumericsLabs2
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

            controller.SetNthIssue(1);
            RefreshIssue();

            TimeSlider.Minimum = 0;
            TimeSlider.Maximum = controller.Issue.Parameters.TopY;
            TimeSlider.TickFrequency = controller.Issue.Parameters.Hy;
            TimeSlider.Value = 0;
            TimeSlider.IsSnapToTickEnabled = true;

            RefreshYErrorGraph();
            RefreshGraph();
            RefreshIterSlider();
            RefreshIterGraph();
            RefreshAnalyticalGraph();
        }

        private void RefreshIssue()
        {
            controller.Refresh();
        }

        private void RefreshGraph()
        {
            GraphVisualizer.Plot.Clear();

            int t = (int)(TimeSlider.Value / controller.Issue.Parameters.Hy);
            GraphVisualizer.Plot.AddScatter(controller.RangeOfX, controller.GetResultByTIndex(t), System.Drawing.Color.Red, 2, 2);
            GraphVisualizer.Plot.AddScatter(controller.RangeOfX, controller.GetAnalyticalResultByTIndex(t), System.Drawing.Color.Blue, 3, 3);
            GraphVisualizer.Plot.XLabel("x");
            GraphVisualizer.Plot.YLabel($"u(x, y={t * controller.Issue.Parameters.Hy})");

            GraphVisualizer.Refresh();
        }
        private void RefreshYErrorGraph()
        {
            ErrorVisualizer.Plot.Clear();
            ErrorVisualizer.Plot.AddScatter(controller.RangeOfY, controller.GetError());
            ErrorVisualizer.Plot.XLabel("y");
            ErrorVisualizer.Plot.YLabel("ε(y)");
            ErrorVisualizer.Refresh();


            ErrorAllVisualizer.Plot.Clear();
            ErrorAllVisualizer.Plot.AddScatter(controller.RangeOfIter, controller.GetIterError());
            ErrorAllVisualizer.Plot.XLabel("y");
            ErrorAllVisualizer.Plot.YLabel("ε(y)");
            ErrorAllVisualizer.Refresh();
        }
        private void RefreshIterGraph()
        {
            HeatVisualizer.Plot.Clear();
            var hm = HeatVisualizer.Plot.AddHeatmap(controller.GetIterationResult((int)IterationSlider.Value), ScottPlot.Drawing.Colormap.Solar);
            
            var cb = HeatVisualizer.Plot.AddColorbar(hm);
            HeatVisualizer.Plot.XLabel("x");
            HeatVisualizer.Plot.YLabel("y");
            HeatVisualizer.Refresh();
        }
        private void RefreshAnalyticalGraph()
        {
            AnanylitcalHeatVisualizer.Plot.Clear();
            var hm = AnanylitcalHeatVisualizer.Plot.AddHeatmap(controller.GetAnalyticalResult(), ScottPlot.Drawing.Colormap.Solar);
            hm.Smooth = true;
            var cb = AnanylitcalHeatVisualizer.Plot.AddColorbar(hm);
            AnanylitcalHeatVisualizer.Plot.XLabel("x");
            AnanylitcalHeatVisualizer.Plot.YLabel("y");
            AnanylitcalHeatVisualizer.Refresh();
            OmegaBlock.Visibility = controller.NeedOmega ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RefreshIterSlider()
        {
            IterationSlider.Minimum = 0;
            IterationSlider.TickFrequency = 1;
            IterationSlider.Maximum = controller.IterationResults.Count - 1;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshGraph();
        }

        private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (controller?.Issue != null)
            {
                controller.SetCalculator(CalcComboBox.SelectedItem.ToString(), OmegaSlider.Value);
                RefreshIssue();
                RefreshYErrorGraph();
                RefreshGraph();
                RefreshIterSlider();
                RefreshIterGraph();

                OmegaBlock.Visibility = controller.NeedOmega ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Iteration_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshIterGraph();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectionChanged(sender, null);
        }

        private void OmegaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeButton.Content = $"Изменить на ω = {OmegaSlider.Value}";
        }
    }
}
