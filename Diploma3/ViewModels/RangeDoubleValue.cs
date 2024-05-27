using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diploma3.ViewModels
{
	public class RangeDoubleValue : INotifyPropertyChanged
	{
		private double initValue;
		private double value;
		private double minValue;
		private double maxValue;

		public RangeDoubleValue(double minValue, double maxValue, double value, double step, string propertyName)
		{
			MinValue = minValue;
			MaxValue = maxValue;
			Value = value;
			initValue = value;
			Step = step;
		}

		public double Value
		{
			get => value;
			set
			{
				var newValue = value;
				newValue = Math.Min(MaxValue, newValue);
				newValue = Math.Max(MinValue, newValue);

				if (this.value != newValue)
				{
					this.value = newValue;
					OnPropertyChanged();
				}
			}
		}

		public double MinValue
		{
			get => minValue;
			set
			{
				if (minValue != value)
				{
					minValue = value;
					OnPropertyChanged();
				}
			}
		}

		public double MaxValue
		{
			get => maxValue;
			set
			{
				if (maxValue != value)
				{
					maxValue = value;
					OnPropertyChanged();
				}
			}
		}
		public double Step { get; }

		public double InitValue
		{
			get => initValue;
			set
			{
				var newValue = value;
				newValue = Math.Min(MaxValue, newValue);
				newValue = Math.Max(MinValue, newValue);

				this.value = newValue;
				initValue = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}


		public event EventHandler? ValueChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string paramName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));

			if (paramName == nameof(Value))
			{
				ValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
