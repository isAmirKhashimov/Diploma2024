using Diploma3.ContoureDetection;
using MahApps.Metro.Actions;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diploma3.ViewModels
{
	/*
	 * image
	 * imageIndex (<- ->)
	 * 
	 * // Contoures selection
	 * thresholdImage = 100 
	 * minPointsCount [3..] (|---------|)
	 * contourIndex (<- ->)
	 * 
	 * // Filiration options
	 * countApproximationDensityLimit = 10 (|-------|)
	 * approximationDensity = 1  [cnt/3, 5] (|-------|)
	 * eps (min delta) = 5 (|---------|) // for ApproximatelyOnLine - to change
	 * 
	 * // Refinition options
	 * density = 0.03 (|----------|) // for Refine
	 */
	public class ContouresSettingsViewModel : INotifyPropertyChanged
	{
		private const int borderColorThickness = 30;
		private readonly Scaler scaler;
		private Bitmap? targetImage;
		private ThresholdType selectedThreshold;
		private TransformType selectedTransform;
		private bool handDrawingEnabled;
		private IEnumerable<ReferencePoint> contoure;

		public ContouresSettingsViewModel(Scaler scaler)
		{
			this.scaler = scaler;

			ImageThreshold = new(minValue: 0, maxValue: 255, step: 1, propertyName: nameof(ImageThreshold));
			MinContourePointsCount = new(minValue: 4, maxValue: 100, value: 4, step: 1, propertyName: nameof(MinContourePointsCount));
			ContoureIndex = new(minValue: 0, maxValue: 0, value: 1, step: 1, propertyName: nameof(ContoureIndex));
			CountApproximationDensityLimit = new(minValue: 1, maxValue: 100, value: 10, step: 1, propertyName: nameof(CountApproximationDensityLimit));
			ApproximationDensityLimit = new(minValue: 1, maxValue: 10, value: 5, step: 1, propertyName: nameof(ApproximationDensityLimit));
			DeviationEps = new(minValue: 0, maxValue: 1, value: 0.15, step: 0.01, propertyName: nameof(DeviationEps));
			ShortestDistance = new(minValue: 0, maxValue: 10, value: 0.2, step: 0.1, propertyName: nameof(ShortestDistance));
			ClosestDistance = new(minValue: 10, maxValue: 50, value: 10, step: 0.5, propertyName: nameof(ClosestDistance));
			Density = new(minValue: 0.01, maxValue: 1, value: 1, step: 0.01, propertyName: nameof(Density));
			GammaCorrection = new(minValue: 0.5, maxValue: 5.0, value: 1.8, step: 0.1, propertyName: nameof(GammaCorrection));
			SmoothLevel = new(minValue: 1, maxValue: 19, value: 3, step: 2, propertyName: nameof(SmoothLevel));

			TriangulationBahdWidth = new(minValue: 0.01, maxValue: 0.1, value: 0.03, step: 0.01, propertyName: nameof(TriangulationBahdWidth));
			TriangulationMinimalDistance = new(minValue: 0.01, maxValue: 0.1, value: 0.03, step: 0.01, propertyName: nameof(TriangulationMinimalDistance));
			TriangulationMinimalDistanceRelaxer = new(minValue: 0.1, maxValue: 1.0, value: 0.65, step: 0.01, propertyName: nameof(TriangulationMinimalDistanceRelaxer));

			ImageThreshold.ValueChanged += ContoureRangeValueChanged;
			MinContourePointsCount.ValueChanged += ContoureRangeValueChanged;
			ContoureIndex.ValueChanged += ContoureRangeValueChanged;
			CountApproximationDensityLimit.ValueChanged += ContoureRangeValueChanged;
			ApproximationDensityLimit.ValueChanged += ContoureRangeValueChanged;
			DeviationEps.ValueChanged += ContoureRangeValueChanged;
			ShortestDistance.ValueChanged += ContoureRangeValueChanged;
			Density.ValueChanged += ContoureRangeValueChanged;
			GammaCorrection.ValueChanged += ContoureRangeValueChanged;
			SmoothLevel.ValueChanged += ContoureRangeValueChanged;
			ClosestDistance.ValueChanged += ContoureRangeValueChanged;

			TriangulationBahdWidth.ValueChanged += TriangulationRangeValueChanged;
			TriangulationMinimalDistance.ValueChanged += TriangulationRangeValueChanged;
			TriangulationMinimalDistanceRelaxer.ValueChanged += TriangulationRangeValueChanged;

			SetImageDefaultValuesCommand = new RelayCommand(SetImageDefaultValues);
			SetSchemaDefaultValuesCommand = new RelayCommand(SetSchemaDefaultValues);
			SetAutoDefaultValuesCommand = new RelayCommand(SetAutoDefaultContoureValues);

			ContouringModeState.PropertyChanged += (_, _) => HandDrawingEnabled = TriangulationModeState.ManualModeOn;
		}


		private void ContoureRangeValueChanged(object? sender, EventArgs e)
		{
			ContoureDataUpdate();
		}

		private void TriangulationRangeValueChanged(object? sender, EventArgs e)
		{
			TriangulationDataUpdate();
		}

		public bool IsContourizationEnabled => TargetImage != null;

		public bool IsTriangulationEnabled => IsContourizationEnabled && Contoure != null && Contoure.Any();

		public BiRangeIntValue ImageThreshold { get; }

		public bool HandDrawingEnabled
		{
			get => handDrawingEnabled;
			set
			{
				if (handDrawingEnabled != value)
				{
					handDrawingEnabled = value;
					OnPropertyChanged();
				}
			}
		}


		public IEnumerable<ThresholdType> ThresholdTypeNames => Enum.GetValues<ThresholdType>();
		public IEnumerable<TransformType> TransformTypeNames => Enum.GetValues<TransformType>();

		public ThresholdType SelectedThreshold
		{
			get => selectedThreshold;
			set
			{
				if (selectedThreshold != value)
				{
					selectedThreshold = value;
					OnPropertyChanged();
					ContoureDataUpdate();
				}
			}
		}


		public TransformType SelectedTransform
		{
			get => selectedTransform;
			set
			{
				if (selectedTransform != value)
				{
					selectedTransform = value;
					OnPropertyChanged();
					ContoureDataUpdate();
				}
			}
		}

		public RangeIntValue SmoothLevel { get; }
		public RangeDoubleValue GammaCorrection { get; }
		public RangeIntValue MinContourePointsCount { get; }
		public RangeIntValue ContoureIndex { get; }

		public RangeIntValue CountApproximationDensityLimit { get; }
		public RangeIntValue ApproximationDensityLimit { get; }
		public RangeDoubleValue DeviationEps { get; }
		public RangeDoubleValue ShortestDistance { get; }
		public RangeDoubleValue ClosestDistance { get; }

		public RangeDoubleValue Density { get; }

		public RangeDoubleValue TriangulationBahdWidth { get; }
		public RangeDoubleValue TriangulationMinimalDistance { get; }
		public RangeDoubleValue TriangulationMinimalDistanceRelaxer { get; }

		public IEnumerable<ReferencePoint> Contoure
		{
			get => contoure;
			set
			{
				if (contoure != value)
				{
					contoure = value;
					OnPropertyChanged(nameof(IsTriangulationEnabled));
				}
			}
		}
		public Bitmap? TargetImage
		{
			get => targetImage;
			set
			{
				if (targetImage != value)
				{
					targetImage = value;
					ContoureDataUpdate();
					OnPropertyChanged(nameof(IsContourizationEnabled));
					OnPropertyChanged(nameof(IsTriangulationEnabled));
				}
			}
		}

		public ModeViewModel ContouringModeState { get; } = new();
		public ModeViewModel TriangulationModeState { get; } = new();

		public ImageSource ImageSource { get; private set; }

		public RelayCommand SetSchemaDefaultValuesCommand { get; }
		public RelayCommand SetImageDefaultValuesCommand { get; }
		public RelayCommand SetAutoDefaultValuesCommand { get; }
		public RelayCommand SetAutoTriangulationValuesCommand { get; }
		/*
        public void AddCurrentIndexOffset(int offset)
        {
            currentIndex += offset;
            currentIndex = Math.Min(currentIndex, contoures.Length - 1);
            currentIndex = Math.Max(currentIndex, 0);
        }
        public void AddApproximationDensityOffset(int offset)
        {
            approximationDensity += offset;
            approximationDensity = Math.Min(approximationDensity, contoures[currentIndex].Length / 3);
            approximationDensity = Math.Max(approximationDensity, 5);
        }
		*/


		public event PropertyChangedEventHandler? PropertyChanged;
		public event EventHandler? ContouresChanged;
		public event EventHandler? TriangulationChanged;

		public void OnPropertyChanged([CallerMemberName] string parameterName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parameterName));
		}

		private void ContoureDataUpdate()
		{
			var imageEditor = new ImageEditor(TargetImage)
				.WithTransform(SelectedTransform)
				.WithSmoothGaussian(SmoothLevel.Value)
				.WithGammaCorrection(GammaCorrection.Value)
				.WithThreshold(SelectedThreshold, (byte)ImageThreshold.LowerValue, (byte)ImageThreshold.UpperValue)
			;

			var contoures = imageEditor.GetContoures(MinContourePointsCount.Value);

			Contoure = Contoures.RemoveClosest(contoures.GetContoure(
				ContoureIndex: ContoureIndex.Value,
				countLimit: CountApproximationDensityLimit.Value,
				approximationDensity: ApproximationDensityLimit.Value,
				eps: DeviationEps.Value,
				distanceLimit: ShortestDistance.Value,
				refineDensity: scaler.TransformBack(Density.Value),
				closestDistance: ClosestDistance.Value), ClosestDistance.Value)
				.Select(el => new ReferencePoint(el));

			ContoureIndex.MaxValue = contoures.Count - 1;
			ContoureIndex.Value = ContoureIndex.Value;

			ApproximationDensityLimit.MaxValue = Contoure.Count() / 3;
			ApproximationDensityLimit.Value = ApproximationDensityLimit.Value;


			using (MemoryStream memory = new MemoryStream())
			{
				imageEditor.GrayBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				ImageSource = bitmapimage;
				OnPropertyChanged(nameof(ImageSource));
			}

			ContouresChanged?.Invoke(this, EventArgs.Empty);
		}


		private void TriangulationDataUpdate()
		{
			TriangulationChanged?.Invoke(this, EventArgs.Empty);
		}


		public void SetAutoTriangulationDefaultValues()
		{
			TriangulationBahdWidth.InitValue = Density.Value / 4;
			TriangulationMinimalDistance.InitValue = Density.Value / 4;
			TriangulationMinimalDistanceRelaxer.InitValue = 0.35;
			TriangulationDataUpdate();
		}
		private void SetSchemaDefaultValues()
		{
			ImageThreshold.InitLowerValue = 0;
			ImageThreshold.InitUpperValue = 255;
			MinContourePointsCount.InitValue = 4;
			ContoureIndex.InitValue = 0;
			CountApproximationDensityLimit.InitValue = 10;
			ApproximationDensityLimit.InitValue = 7;
			DeviationEps.InitValue = 0.15;
			ShortestDistance.InitValue = 0.2;
			ClosestDistance.InitValue = 13.428571428571427;
			Density.InitValue = 0.1;
			GammaCorrection.InitValue = 3.2164179;//2.3285714285714292; // 2.6856;
			SmoothLevel.InitValue = 1;
			selectedThreshold = ThresholdType.BinaryInv;
			selectedTransform = TransformType.None;

			OnPropertyChanged(nameof(SelectedThreshold));
			OnPropertyChanged(nameof(SelectedTransform));
			ContoureDataUpdate();
		}
		/*
		 * ImageThreshold					= new(minValue: 0, maxValue: 255, step: 1, propertyName: nameof(ImageThreshold));
		 * MinContourePointsCount			= new(minValue: 4, maxValue: 100, value: 4, step: 1, propertyName: nameof(MinContourePointsCount));
		 * ContoureIndex					= new(minValue: 0, maxValue: 0, value: 1, step: 1, propertyName: nameof(ContoureIndex));
		 * CountApproximationDensityLimit	= new(minValue: 1, maxValue: 100, value: 10, step: 1, propertyName: nameof(CountApproximationDensityLimit));
		 * ApproximationDensityLimit		= new(minValue: 1, maxValue: 10, value: 5, step: 1, propertyName: nameof(ApproximationDensityLimit));
		 * DeviationEps					= new(minValue: 0, maxValue: 1, value: 0.15, step: 0.01, propertyName: nameof(DeviationEps));
		 * ShortestDistance				= new(minValue: 0, maxValue: 10, value: 0.2, step: 0.1, propertyName: nameof(ShortestDistance));
		 * Density							= new(minValue: 0.1, maxValue: 1, value: 1, step: 0.01, propertyName: nameof(Density));
		 * GammaCorrection					= new(minValue: 0.5, maxValue: 3.5, value: 1.8, step: 0.1, propertyName: nameof(GammaCorrection));
		 * SmoothLevel						= new(minValue: 1, maxValue: 11, value: 3, step: 2, propertyName: nameof(SmoothLevel));
		 */
		private void SetImageDefaultValues()
		{
			ImageThreshold.InitLowerValue = 16;
			ImageThreshold.InitUpperValue = 255;
			MinContourePointsCount.InitValue = 4;
			ContoureIndex.InitValue = 0;
			CountApproximationDensityLimit.InitValue = 14;
			ApproximationDensityLimit.InitValue = 5;
			DeviationEps.InitValue = 0.13809523809523808;
			ShortestDistance.InitValue = 5.5095238095238122;
			ClosestDistance.InitValue = 13.428571428571427;
			Density.InitValue = 0.1;
			GammaCorrection.InitValue = 3.5;//1.8;
			SmoothLevel.InitValue = 19;
			selectedThreshold = ThresholdType.BinaryInv;
			selectedTransform = TransformType.None;

			OnPropertyChanged(nameof(SelectedThreshold));
			OnPropertyChanged(nameof(SelectedTransform));
			ContoureDataUpdate();
		}


		public void SetAutoDefaultContoureValues()
		{
			ImageThreshold.InitLowerValue = 16;
			ImageThreshold.InitUpperValue = 255;
			MinContourePointsCount.InitValue = 4;
			ContoureIndex.InitValue = 0;
			CountApproximationDensityLimit.InitValue = 14;
			ApproximationDensityLimit.InitValue = 5;
			DeviationEps.InitValue = 0.13809523809523808;
			ShortestDistance.InitValue = 5.5095238095238122;
			ClosestDistance.InitValue = 13.428571428571427;
			Density.InitValue = 0.1;
			GammaCorrection.InitValue = 1.8;
			SmoothLevel.InitValue = 19;
			selectedThreshold = ThresholdType.Binary;
			selectedTransform = TransformType.None;

			var imageEditor = new ImageEditor(TargetImage)
				.WithTransform(SelectedTransform)
				.WithSmoothGaussian(SmoothLevel.Value)
				.WithGammaCorrection(GammaCorrection.Value)
				.WithThreshold(SelectedThreshold, (byte)ImageThreshold.LowerValue, (byte)ImageThreshold.UpperValue);

			/*
			var bitmap = imageEditor.GrayBitmap;

			var colorCount = 0;

			for (int x = 0; x < bitmap.Width - borderColorThickness; x++)
			{
				for (int y = 0; y < borderColorThickness; y++)
				{
					var px = bitmap.GetPixel(x, y);
					colorCount += px.R + px.G + px.B - 382;
				}

				for (int y = bitmap.Height - borderColorThickness; y < bitmap.Height; y++)
				{
					var px = bitmap.GetPixel(x, y);
					colorCount += px.R + px.G + px.B - 382;
				}
			}

			if (colorCount > 0)
			{
				SetSchemaDefaultValues();
			}
			else
			{
				SetImageDefaultValues();
			}*/

			var bitmap = imageEditor.Bitmap;

			var nongrayCount = 0;
			var difColors = new HashSet<System.Drawing.Color>();

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < borderColorThickness; y++)
				{
					var px = bitmap.GetPixel(x, y);
					difColors.Add(px);
					nongrayCount += (Math.Abs(px.R - px.G) + Math.Abs(px.R - px.B) + Math.Abs(px.B - px.G)) == 0 ? 0 : 1;
				}

				for (int y = bitmap.Height - borderColorThickness; y < bitmap.Height; y++)
				{
					var px = bitmap.GetPixel(x, y);
					difColors.Add(px);
					nongrayCount += (Math.Abs(px.R - px.G) + Math.Abs(px.R - px.B) + Math.Abs(px.B - px.G)) == 0 ? 0 : 1;
				}
			}

			if (difColors.Count < 4 || nongrayCount < 4)
			{
				SetSchemaDefaultValues();
			}
			else
			{
				SetImageDefaultValues();
			}
		}

	}
}
