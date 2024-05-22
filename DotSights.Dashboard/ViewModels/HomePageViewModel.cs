using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using DotSights.Core;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using DotSights.Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DotSights.Dashboard.ViewModels
{
	public partial class HomePageViewModel : ViewModelBase
	{
		[ObservableProperty]
		private string _searchQuery = "";

		[ObservableProperty]
		private Bitmap? _hourlyUseImage;
		[ObservableProperty]
		private Bitmap? _focusedTimeImage;

		[ObservableProperty]
		private string _totalTimeToday = "";
		[ObservableProperty]
		private string _totalTimeYersteday = "";
		[ObservableProperty]
		private string _totalTimeThisWeek = "";

		public ObservableCollection<ActivityData> ListItems { get => _listItems; set => this.SetProperty(ref _listItems, value); }
		private ObservableCollection<ActivityData> _listItems = new();

		private List<ActivityData> data = new();

		private static string FolderPath = Environment.CurrentDirectory;
		private const string hourlyuseplotfilename = "HourlyTimeUsagePlot.png";
		private const string foucsedtimeplotfilename = "FocusedTimePlot.png";

		public HomePageViewModel()
		{
			ConfigurationService.Instance.LoadSettings();
			var service = new ActivityDataService();
			var activities = service.GetActivityData().ToList();
			if (activities == null || activities.Count == 0)
				return;

			// Initialize ListItems using the new keyword
			_listItems = new ObservableCollection<ActivityData>(activities);
			data = activities;

			SearchCommand();

			Core.DotSights.CreateDataCharts(activities);

			HourlyUseImage = new Bitmap(Path.Combine(FolderPath, hourlyuseplotfilename));
			FocusedTimeImage = new Bitmap(Path.Combine(FolderPath, foucsedtimeplotfilename));

			TotalTimeToday = DotFormatting.FormatTimeLong(activities.Sum(x => x.TotalTimeToday));
			TotalTimeYersteday = DotFormatting.FormatTimeLong(activities.Sum(x => x.Last7DaysUsage.ContainsKey(DateTime.Today.AddDays(-1)) ? x.Last7DaysUsage[DateTime.Today.AddDays(-1)] : 0));
			TotalTimeThisWeek = DotFormatting.FormatTimeLong(activities.Sum(x => x.Last7DaysUsage.Where(e => (DateTime.Now - e.Key).TotalDays < 7).Select(e => e.Value).Sum()));
		}

		public void SearchCommand()
		{
			var result = Core.DotSights.FilterDataFromSettings(data, ConfigurationService.Instance.DashboardSettings);
			if (string.IsNullOrEmpty(SearchQuery))
			{
				ListItems = new ObservableCollection<ActivityData>(result);
				return;
			}

			ListItems = new ObservableCollection<ActivityData>(result.Where(x => x.WindowTitle.Contains(SearchQuery)));
		}
	}
}
