using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotSights.Core;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using DotSights.Dashboard.Models;
using DotSights.Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Transactions;

namespace DotSights.Dashboard.ViewModels
{
	public partial class HomePageViewModel : ViewModelBase
	{
		private static HomePageViewModel? _instance;
		public static HomePageViewModel Instance => _instance ??= new HomePageViewModel();

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

		[ObservableProperty]
		private ObservableCollection<ActivityDataWrapper> _listItems;

		private List<ActivityData> data = new();

		private static string FolderPath = Environment.CurrentDirectory;
		private const string hourlyuseplotfilename = "HourlyTimeUsagePlot.png";
		private const string foucsedtimeplotfilename = "FocusedTimePlot.png";
		private const string dataChart1 = "DataChart1.png";

		public HomePageViewModel()
		{
			ConfigurationService.Instance.LoadSettings();
			var service = new ActivityDataService();
			var activities = service.GetActivityData().ToList();
			if (activities == null || activities.Count == 0)
				return;

			ListItems = new ObservableCollection<ActivityDataWrapper>(activities.Select(e => new ActivityDataWrapper(e)));
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
			var result = Core.DotSights.FilterDataFromSettings(data, Core.DotSights.LoadSettings());
			if (string.IsNullOrEmpty(SearchQuery))
			{
				ListItems = new ObservableCollection<ActivityDataWrapper>(result.Select(e => new ActivityDataWrapper(e)));
				return;
			}

			ListItems = new ObservableCollection<ActivityDataWrapper>(result.Where(x => x.WindowTitle.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase)).Select(e => new ActivityDataWrapper(e)));
		}

		[RelayCommand]
		public void SelectDataCommand(ActivityDataWrapper wrapper)
		{
			if(!wrapper.Selected)
			{
				Core.DotSights.CreateDataChartForActivity(wrapper.Data);
				wrapper.FetchCharts();
			}
			List<ActivityDataWrapper> list = new List<ActivityDataWrapper>();
			foreach (var item in ListItems)
			{
				item.Selected = item == wrapper ? !item.Selected : false;
				list.Add(item);
			}
			ListItems = new ObservableCollection<ActivityDataWrapper>(list);
		}

	}

	public partial class ActivityDataWrapper : ObservableObject
	{
		public ActivityData Data { get; set; }
		public bool Selected { get; set; } = false;

		public Bitmap ActiveHoursChart { get; set; } 
		public Bitmap ActiveDaysChart { get; set; } 

		public ActivityDataWrapper(ActivityData data)
		{
			Data = data;
		}

		[RelayCommand]
		public void Foo()
		{
			HomePageViewModel.Instance.SearchQuery = Data.WindowTitle;
		}

		public void FetchCharts()
		{
			ActiveHoursChart = new Bitmap(Path.Combine(Environment.CurrentDirectory, "ActiveHours.png"));
			ActiveDaysChart = new Bitmap(Path.Combine(Environment.CurrentDirectory, "ActiveDays.png"));
		}
	}
}
