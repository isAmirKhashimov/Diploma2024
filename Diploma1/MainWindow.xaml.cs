using System.Windows;

namespace Diploma1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new Controller().Calculate();
        }
    }
}