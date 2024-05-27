using Xceed.Wpf.Toolkit;
namespace Diploma3
{
	public class BusyIndicatorController(BusyIndicator indicator)
	{
		public void SetBusy()
		{
			indicator.IsBusy = true;
		}

		public void SetBusyContent(string text)
		{
			indicator.BusyContent = text;
		}

		public void SetNotBusy()
		{
			indicator.IsBusy = false;
		}
	}
}
