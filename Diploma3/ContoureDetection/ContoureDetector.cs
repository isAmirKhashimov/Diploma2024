using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace Diploma3.ContoureDetection
{
	internal class ImageEditor
	{
		public ImageEditor(Bitmap bitmap)
		{
			SourceImage = bitmap.ToImage<Bgr, byte>();
			ResImage = SourceImage.Convert<Gray, byte>();
		}

		public ImageEditor WithGammaCorrection(double gammaCorrection)
		{
			ResImage._EqualizeHist();
			ResImage._GammaCorrect(gammaCorrection);
			return this;
		}

		public Bitmap Bitmap => SourceImage.ToBitmap();
		public Bitmap GrayBitmap => ResImage.ToBitmap();

		public string FileName { get; }

		public Image<Bgr, byte> SourceImage { get; private set; }

		private Image<Gray, byte> ResImage { get; set; }

		public ImageEditor WithSmoothGaussian(int smoothLevel)
		{
			if (smoothLevel % 2 == 0) throw new ArgumentException("Только нечетные значения");

			ResImage = ResImage.SmoothGaussian(smoothLevel);
			return this;
		}

		public ImageEditor WithThreshold(ThresholdType thresholdType, byte minThreshold, byte maxThreshold)
		{
			ResImage = thresholdType switch
			{
				ThresholdType.Binary => ResImage.ThresholdBinary(new Gray(minThreshold), new Gray(maxThreshold)),
				ThresholdType.BinaryInv => ResImage.ThresholdBinaryInv(new Gray(minThreshold), new Gray(maxThreshold)),
				ThresholdType.ToZero => ResImage.ThresholdToZero(new Gray(minThreshold)),
				ThresholdType.ToZeroInv => ResImage.ThresholdToZeroInv(new Gray(minThreshold)),
				_ => throw new NotImplementedException(),
			};
			return this;
		}

		public ImageEditor WithTransform(TransformType transformType)
		{
			return transformType switch
			{
				TransformType.None => this,
				TransformType.Canny => WithCannyTransform(),
				TransformType.Sobel => WithSobelTransform(3),
				TransformType.Laplace => WithLaplaceTransform(3),
				_ => throw new NotImplementedException(),
			};
		}

		private ImageEditor WithCannyTransform()
		{
			ResImage = ResImage.Canny(50, 100);
			return this;
		}

		private ImageEditor WithSobelTransform(int apertureSize)
		{
			ResImage = ResImage.Sobel(1, 0, apertureSize).Convert<Gray, byte>();
			return this;
		}

		private ImageEditor WithLaplaceTransform(int apertureSize)
		{
			ResImage = ResImage.Laplace(apertureSize).Convert<Gray, byte>();
			return this;
		}

		public Contoures GetContoures(int minimalPointsCount)
		{
			var contours = new VectorOfVectorOfPoint();
			var hierarchy = new Mat();

			CvInvoke.FindContours(ResImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

			return new(contours.ToArrayOfArray().Where(ar => ar.Length >= minimalPointsCount).ToArray());
			//CvInvoke.DrawContours(Image, contours, 20, new MCvScalar(255, 0, 0), 3);
		}
	}


	public enum TransformType
	{
		None,
		Canny,
		Sobel,
		Laplace,
	}

	public enum ThresholdType
	{
		Binary,
		BinaryInv,
		ToZero,
		ToZeroInv
	}
}
