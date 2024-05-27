using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PdfImageController
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			
			

			RenderTargetBitmap renderTargetBitmap =
	new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(canvasic);
			PngBitmapEncoder pngImage = new PngBitmapEncoder();
			pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			using (Stream fileStream = File.Create("out.png"))
			{
				pngImage.Save(fileStream);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();

			if (dialog.ShowDialog() == true)
			{
				//var bitmap = editor.Bitmap.SetOpacity(0.4f);
				var bitmap = System.Drawing.Image.FromFile(вialog.FileName).SetOpacity(0.4f);
				var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				bitmap.Dispose();

				var im = new Image()
				{
					Source = bitmapSource,
				};

				InitField();
				PlayGroundCanvas.Children.Add(im);
				Canvas.SetLeft(im, 0);
				Canvas.SetTop(im, 0);
			}
		}
	}
}