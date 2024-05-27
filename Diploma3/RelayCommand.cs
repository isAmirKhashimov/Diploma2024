using System.Windows.Input;

namespace Diploma3
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> action;

		public RelayCommand(Action<object> action)
		{
			this.action = action;
		}
		public RelayCommand(Action action) : this((_) => action())
		{

		}
#pragma warning disable 0067
		public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			action(parameter);
		}
	}
}
