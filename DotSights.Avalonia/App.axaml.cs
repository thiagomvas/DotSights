using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DotSights.Avalonia.ViewModels;
using DotSights.Avalonia.Views;
using ScottPlot;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;

namespace DotSights.Avalonia
{
	public partial class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainWindowViewModel(),
				};
			}

			base.OnFrameworkInitializationCompleted();
		}


		public static void CreateDataCharts(List<ActivityData> data)
		{

			if (data == null || data.Count == 0)
				return;

			Plot plot = new();

			var values = data.Select(x => (double)x.FocusedTimeInSeconds).ToArray();

			var gauge = plot.Add.RadialGaugePlot(values);

			plot.FigureBackground.Color = Color.FromHex("#00000000");

			plot.SavePng(Path.Combine(Environment.CurrentDirectory, "FocusedTimePlot.png"), 540, 540);

			Plot plot2 = new();

			var hourlyUsageInSeconds = Enumerable.Range(0, 24).Select(hour => (double)data.Sum(e => e.UsageTimePerHour.ContainsKey(hour) ? e.UsageTimePerHour[hour] : 0)).ToArray();

			double[] plotValues = new double[24];
			if(hourlyUsageInSeconds.Any(x => x >= 3600))
			{
				plotValues = hourlyUsageInSeconds.Select(x => x / 3600).ToArray();
			}
			else if(hourlyUsageInSeconds.Any(x => x >= 60))
			{
				plotValues = hourlyUsageInSeconds.Select(x => x / 60).ToArray();
			}
			else
			{
				plotValues = hourlyUsageInSeconds;
			}

			var barPlot = plot2.Add.Bars(Enumerable.Range(0, 24).Select(e => (double) e).ToArray(), plotValues);
			plot2.FigureBackground.Color = Color.FromHex("#00000000");
			plot2.Axes.Color(Color.FromHex("#FFFFFF"));

			barPlot.ValueLabelStyle.ForeColor = Color.FromHex("#FFFFFF");
			barPlot.Color = Color.FromHex("#F68A06");

			var bars = barPlot.Bars.ToList();
			for(int i = 0; i < 24; i++)
			{
				bars[i].Label = DotFormatting.FormatTime((int) hourlyUsageInSeconds[i]);
				bars[i].BorderLineWidth = 0;
			}
			

			plot2.SavePng("HourlyTimeUsagePlot.png", 1080, 540);

			Plot plot3 = new();

			double[] vals = [1, 2, 3, 4, 5];
			plot3.Add.Scatter(vals, vals);

			plot3.SaveBmp("ScatterPlot.bmp", 1920, 540);


		}
	}
}