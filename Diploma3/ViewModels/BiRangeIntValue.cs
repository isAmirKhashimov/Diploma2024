using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diploma3.ViewModels
{
	public class BiRangeIntValue : INotifyPropertyChanged
	{
		private int lowerValue;
		private int upperValue;
		private int initUpperValue;
		private int minValue;
		private int maxValue;
		private int initLowerValue;

		public BiRangeIntValue(int minValue, int maxValue, int step, string propertyName)
		{
			MinValue = minValue;
			MaxValue = maxValue;
			lowerValue = minValue;
			upperValue = maxValue;
			initLowerValue = minValue;
			initUpperValue = maxValue;
			Step = step;
		}

		public int LowerValue
		{
			get => lowerValue;
			set
			{
				var newValue = value;
				newValue = Math.Min(UpperValue, newValue);
				newValue = Math.Max(MinValue, newValue);

				if (lowerValue != newValue)
				{
					lowerValue = newValue;
					OnPropertyChanged();
				}
			}
		}

		public int UpperValue
		{
			get => upperValue;
			set
			{
				var newValue = value;
				newValue = Math.Min(MaxValue, newValue);
				newValue = Math.Max(LowerValue, newValue);

				if (upperValue != newValue)
				{
					upperValue = newValue;
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


		public int InitLowerValue
		{
			get => initLowerValue;
			set
			{
				var newValue = value;
				newValue = Math.Min(MaxValue, newValue);
				newValue = Math.Max(MinValue, newValue);

				lowerValue = newValue;
				initLowerValue = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LowerValue)));
			}
		}

		public int InitUpperValue
		{
			get => initUpperValue;
			set
			{
				var newValue = value;
				newValue = Math.Min(MaxValue, newValue);
				newValue = Math.Max(MinValue, newValue);

				upperValue = newValue;
				initUpperValue = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpperValue)));
			}
		}

		public event EventHandler? ValueChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string paramName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));

			if (paramName == nameof(LowerValue) || paramName == nameof(UpperValue))
			{
				ValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
