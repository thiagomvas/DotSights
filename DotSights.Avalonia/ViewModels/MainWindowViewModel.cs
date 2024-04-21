using DotSights.Avalonia.Services;

namespace DotSights.Avalonia.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
#pragma warning disable CA1822 // Mark members as static
		public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static


		public MainWindowViewModel()
		{
			var service = new ActivityDataService();
			HomePage = new HomePageViewModel(service.GetActivityData());
		}

		public HomePageViewModel HomePage { get; }
	}
}
