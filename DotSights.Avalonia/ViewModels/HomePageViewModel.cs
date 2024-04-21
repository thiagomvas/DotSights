using DotSights.Core.Common.Types;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotSights.Avalonia.ViewModels
{
	public class HomePageViewModel : ViewModelBase
	{
		public HomePageViewModel(IEnumerable<ActivityData> activities)
		{
			// Initialize ListItems using the new keyword
			ListItems = new ObservableCollection<ActivityData>(activities);
		}


        public ObservableCollection<ActivityData> ListItems { get; }
	}
}
