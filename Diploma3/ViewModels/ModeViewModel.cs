using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Diploma3.ViewModels
{
	public class ModeViewModel : INotifyPropertyChanged
	{
		private bool automaticModeOn;
		private bool semiAutomaticModeOn;
		private bool manualModeOn;

		public event PropertyChangedEventHandler? PropertyChanged;

		public bool AutomaticModeOn
		{
			get => automaticModeOn;
			set
			{
				if (automaticModeOn != value)
				{
					automaticModeOn = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(HyperParametersVisibility));

					if (value == true)
					{
						SemiAutomaticModeOn = false;
						ManualModeOn = false;
					}
				}
			}
		}

		public bool SemiAutomaticModeOn
		{
			get => semiAutomaticModeOn;
			set
			{
				if (semiAutomaticModeOn != value)
				{
					semiAutomaticModeOn = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(HyperParametersVisibility));

					if (value == true)
					{
						AutomaticModeOn = false;
						ManualModeOn = false;
					}
				}
			}
		}

		public bool ManualModeOn
		{
			get => manualModeOn;
			set
			{
				if (manualModeOn != value)
				{
					manualModeOn = value;
					OnPropertyChanged();

					if (value == true)
					{
						AutomaticModeOn = false;
						SemiAutomaticModeOn = false;
					}
				}
			}
		}

		public Visibility HyperParametersVisibility => (AutomaticModeOn || SemiAutomaticModeOn) ? Visibility.Visible : Visibility.Collapsed;

		public void OnPropertyChanged([CallerMemberName]string paramName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
		}
	}
}
