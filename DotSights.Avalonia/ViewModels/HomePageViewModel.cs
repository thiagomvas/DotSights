using Avalonia;
using Avalonia.Media.Imaging;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotSights.Avalonia.ViewModels
{
	public class HomePageViewModel : ViewModelBase
	{
		public static AvaPlot? plot;
		public static AvaPlot? hourlyusageplot;

		public string SearchQuery { get; set; } = "";
		public Bitmap? HourlyUseImage { get; private set; } 
		public Bitmap? FocusedTimeImage { get; private set; }

		public string TotalTimeToday { get; set; } = "";

		private static string FolderPath = Environment.CurrentDirectory;
		private const string hourlyuseplotfilename = "HourlyTimeUsagePlot.png";
		private const string foucsedtimeplotfilename = "FocusedTimePlot.png";
		private List<ActivityData> data = new();
		public HomePageViewModel(IEnumerable<ActivityData> activities)
		{
			// Initialize ListItems using the new keyword
			ListItems = new ObservableCollection<ActivityData>(activities);
			data = activities.ToList();

			App.CreateDataCharts(activities.ToList());

			HourlyUseImage = new Bitmap(Path.Combine(FolderPath, hourlyuseplotfilename));
			FocusedTimeImage = new Bitmap(Path.Combine(FolderPath, foucsedtimeplotfilename));

			TotalTimeToday = DotFormatting.FormatTimeLong(activities.Sum(x => x.TotalTimeToday));
		}


        public ObservableCollection<ActivityData> ListItems { get => listItems; set => this.RaiseAndSetIfChanged(ref listItems, value); }
		private ObservableCollection<ActivityData> listItems;

		public void SearchCommand()
		{
			if (string.IsNullOrEmpty(SearchQuery))
			{
				ListItems = new ObservableCollection<ActivityData>(data);
				return;
			}

			ListItems = new ObservableCollection<ActivityData>(data.Where(x => x.WindowTitle.Contains(SearchQuery)));
		}
	}
}
