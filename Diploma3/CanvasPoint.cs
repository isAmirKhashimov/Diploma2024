using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Diploma3
{

	public class CanvasPoint(Ellipse ellipse, ReferencePoint currentPoint, Canvas parent)
	{
		private readonly Canvas parent = parent;
		private bool isCaptured;
		private bool isValueSame = false;

		public Ellipse? Ellipse { get; private set; } = ellipse;
		public ReferencePoint CurrentPoint { get; } = currentPoint;
		public bool IsCaptured 
		{ 
			get => isCaptured;
			private set
			{
				if (isCaptured != value)
				{
					isCaptured = value;
					IsCapturedChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler IsCapturedChanged;
		public event EventHandler ValueChanged;
		public event EventHandler PointRemoved;


		public void MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				IsCaptured = true;
				isValueSame = true;
			}
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && IsCaptured)
			{/*
				Canvas.SetLeft(Ellipse, e.GetPosition(parent).X - Ellipse.Width / 2.0);
				Canvas.SetTop(Ellipse, e.GetPosition(parent).Y - Ellipse.Height / 2.0);*/

				var pos = e.GetPosition(parent);

				if (CurrentPoint.Value != pos)
				{
					CurrentPoint.Value = pos;
					ValueChanged?.Invoke(this, EventArgs.Empty);
					isValueSame = false;
				}
			}
		}

		public void MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released)
			{
				if (isCaptured && isValueSame)
				{
					parent.Children.Remove(Ellipse);
					Ellipse = null;
					PointRemoved?.Invoke(this, EventArgs.Empty);
				}

				IsCaptured = false;
			}
		}
	}
}