using System.Drawing.Imaging;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;

namespace Diploma3
{
	internal static class Extensions
	{
		public static Delaune.Point ToDPoint(this System.Windows.Point point) => new(point.X, point.Y);
		public static Delaune.Point ToDPoint(this System.Drawing.Point point) => new(point.X, point.Y);
		public static System.Windows.Point ToPoint(this Delaune.IPoint point) => new(point.X, point.Y);
		public static System.Windows.Point ToPoint(this System.Drawing.Point point) => new(point.X, point.Y);

		public static Bitmap SetOpacity(this Image image, float opacity)
		{
			try
			{
				//create a Bitmap the size of the image provided  
				var bmp = new Bitmap(image.Width, image.Height);

				//create a graphics object from the image  
				using (Graphics gfx = Graphics.FromImage(bmp))
				{

					//create a color matrix object  
					ColorMatrix matrix = new ColorMatrix();

					//set the opacity  
					matrix.Matrix33 = opacity;

					//create image attributes  
					ImageAttributes attributes = new ImageAttributes();

					//set the color(opacity) of the image  
					attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

					//now draw the image  
					gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
				}
				return bmp;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return null;
			}
		}
		public static BitmapImage ToBitmapImage(this Bitmap bitmap)
		{
			using (var memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();

				return bitmapImage;
			}
		}
	}
}
