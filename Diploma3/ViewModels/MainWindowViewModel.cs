using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diploma3.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private WorkObject currentWorkObject;

		public ObservableCollection<WorkObject> Album { get; } = [];

		public WorkObject CurrentWorkObject
		{
			get => currentWorkObject;
			set
			{
				if (currentWorkObject != value)
				{
					currentWorkObject = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public void OnPropertyChanged([CallerMemberName]string paramName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
		}
	}
}
