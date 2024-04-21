using Avalonia;
using Avalonia.Media.Imaging;
using DotSights.Core.Common.Types;
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

		public Bitmap? HourlyUseImage { get; private set; } 
		public Bitmap? FocusedTimeImage { get; private set; } 

		private static string FolderPath = Environment.CurrentDirectory;
		private const string hourlyuseplotfilename = "HourlyTimeUsagePlot.png";
		private const string foucsedtimeplotfilename = "FocusedTimePlot.png";
		public HomePageViewModel(IEnumerable<ActivityData> activities)
		{
			// Initialize ListItems using the new keyword
			ListItems = new ObservableCollection<ActivityData>(activities);

			App.CreateDataCharts(activities.ToList());

			HourlyUseImage = new Bitmap(Path.Combine(FolderPath, hourlyuseplotfilename));
			FocusedTimeImage = new Bitmap(Path.Combine(FolderPath, foucsedtimeplotfilename));

		}


        public ObservableCollection<ActivityData> ListItems { get; }
	}
}
