using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diploma3.ViewModels
{
	public class RangeIntValue : INotifyPropertyChanged
	{
		private int initValue;
		private int value;
		private int minValue;
		private int maxValue;

		public RangeIntValue(int minValue, int maxValue, int value, int step, string propertyName)
		{
			MinValue = minValue;
			MaxValue = maxValue;
			Value = value;
			Step = step;
			initValue = value;
		}

		public int Value
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
		public string PropertyName { get; }

		public int MinValue
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

		public int MaxValue
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
		public int Step { get; }


		public int InitValue
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
