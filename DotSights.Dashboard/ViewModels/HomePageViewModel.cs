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
			TotalTimeThisWeek = DotFormatting.FormatTimeLong(activities.Sum(x => x.Last7DaysUsage.Values.Sum()));
		}

		public void SearchCommand()
		{
			List<ActivityData> baseData = data;
			var settings = ConfigurationService.Instance.DashboardSettings;

			// Group data with the same process name together
			if (settings.GroupItemsWithSameProcessName)
			{
				baseData = baseData.GroupBy(x => x.ProcessName).Select(x => new ActivityData
				{
					WindowTitle = x.First().WindowTitle,
					FocusedTimeInSeconds = x.Sum(y => y.FocusedTimeInSeconds),
					UsageTimePerWeekDay = x.SelectMany(y => y.UsageTimePerWeekDay).GroupBy(y => y.Key).ToDictionary(y => y.Key, y => y.Sum(z => z.Value)),
					UsageTimePerHour = x.SelectMany(y => y.UsageTimePerHour).GroupBy(y => y.Key).ToDictionary(y => y.Key, y => y.Sum(z => z.Value)),
					UsageTimePerMonth = x.SelectMany(y => y.UsageTimePerMonth).GroupBy(y => y.Key).ToDictionary(y => y.Key, y => y.Sum(z => z.Value)),
					Alias = x.First().Alias,
					ProcessName = x.First().ProcessName,
					Last7DaysUsage = x.SelectMany(y => y.Last7DaysUsage).GroupBy(y => y.Key).ToDictionary(y => y.Key, y => y.Sum(z => z.Value))
				}).ToList();
			}

			if(settings.GroupItemsUsingGroupingRules && settings.GroupingRules.Count > 0)
			{
				// Group by using each of the regex patterns and make the Window name be the grouping rule name
				foreach (var rule in settings.GroupingRules)
				{
					baseData = ActivityData.GroupByRule(baseData, rule);
				}
			}
			


			if (string.IsNullOrEmpty(SearchQuery))
			{
				ListItems = new ObservableCollection<ActivityData>(baseData);
				return;
			}

			ListItems = new ObservableCollection<ActivityData>(baseData.Where(x => x.WindowTitle.Contains(SearchQuery)));
		}
	}
}
