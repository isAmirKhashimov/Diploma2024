using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Diploma3
{
	public class CanvasTransformer(Point point)
	{
		private bool isCaptured = false;
		private Point capturedPoint;
		private Point focusedPoint;
		private double centerX = point.X;
		private double centerY = point.Y;
		private double scale = 1.0;

		public CanvasTransformer() : this(new Point(0, 0))
		{ }

		public bool IsCaptured => isCaptured;

		public double CenterX
		{
			get => centerX;
			private set
			{
				if (centerX != value)
				{
					centerX = value;
					OnPropertyChanged();
				}
			}
		}

		public double CenterY
		{
			get => centerY;
			private set
			{
				if (centerY != value)
				{
					centerY = value;
					OnPropertyChanged();
				}
			}
		}

		public double Scale
		{
			get => scale;
			private set
			{
				if (scale != value)
				{
					scale = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public void ButtonDown(Point point)
		{
			isCaptured = true;
			capturedPoint = point;
			focusedPoint = point;
		}

		public void ButtonUp(Point point)
		{
			isCaptured = false;
			focusedPoint = point;
		}

		public void MouseMoveTo(Point point)
		{
			focusedPoint = point;

			if (isCaptured)
			{
				CenterX += focusedPoint.X - capturedPoint.X;
				CenterY += focusedPoint.Y - capturedPoint.Y;
			}
		}

		private void OnPropertyChanged([CallerMemberName] string paramName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
		}
	}
}
