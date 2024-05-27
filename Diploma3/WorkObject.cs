using Diploma3.ViewModels;
using System.Drawing;
using System.Windows.Media.Imaging;
using DPoint = Delaune.Point;
using WPoint = System.Windows.Point;

namespace Diploma3
{
	public class WorkObject
	{
		public WorkObject(Bitmap previewImageBitmap)
		{
			ImageBitmap = previewImageBitmap;
			PreviewBitmapImage = previewImageBitmap.ToBitmapImage();
		}


		public Bitmap ImageBitmap { get; }

		public BitmapImage PreviewBitmapImage { get; }

		public ContouresSettingsViewModel ContouresSettingsViewModel { get; } = new(new Scaler());

		public HashSet<CanvasInnerPoint> InnerPoints { get; set; } = [];

		public LinkedList<ReferencePoint> OuterPoints { get; set; } = new LinkedList<ReferencePoint>();
	}
}
