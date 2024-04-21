using Avalonia.Controls;
using ScottPlot;
using System.Linq;

namespace DotSights.Avalonia.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public void CreateDataCharts()
		{
			var data = DotSights.Core.DotSights.GetDataFromDataPath();

			if (data == null)
				return;

			Plot plot = new();

			var values = data.Select(x => (double) x.FocusedTimeInSeconds).ToArray();

			plot.Add.RadialGaugePlot(values);

			plot.SaveBmp("FocusedTimePlot.bmp", 1080, 1080);

			Plot plot2 = new();

			var hourlyUsage = Enumerable.Range(0, 24).Select(hour => (double) data.Sum(e => e.UsageTimePerHour.ContainsKey(hour) ? e.UsageTimePerHour[hour] : 0)).ToArray();

			plot2.Add.Bars(hourlyUsage);

			plot2.SaveBmp("HourlyTimeUsagePlot.bmp", 1920, 540);
		}
	}
}