using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using DotSights.Dashboard.Models;
using ScottPlot;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DotSights.Core;

public static class DotSights
{
	public static string DataFilePath { get; set; } = "data.json";

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowText(IntPtr hWnd, string lpString, int nMaxCount); 

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


	public static string GetFocusedWindow()
	{
		IntPtr foregroundWindowHandle = GetForegroundWindow();
		if (foregroundWindowHandle != IntPtr.Zero)
		{
			const int nChars = 256;
			string windowTitle = new string(' ', nChars);
			GetWindowText(foregroundWindowHandle, windowTitle, nChars);
			return windowTitle.Substring(0, windowTitle.IndexOf('\0'));
		}
		return "";
	}

	public static string GetFocusedProcessName()
	{
		IntPtr foregroundWindowHandle = GetForegroundWindow();
		if (foregroundWindowHandle != IntPtr.Zero)
		{
			GetWindowThreadProcessId(foregroundWindowHandle, out uint processId);
			Process process = Process.GetProcessById((int)processId);
			return process.ProcessName;
		}
		return "";
	}

	public static void SaveSettings(DotSightsSettings settings)
	{
		File.WriteAllText("settings.json", JsonSerializer.Serialize(settings, typeof(DotSightsSettings), DotSightSettingsGenerationContext.Default));
	}

	public static DotSightsSettings LoadSettings()
	{
		if (File.Exists("settings.json"))
		{
			string data = File.ReadAllText("settings.json");
			return JsonSerializer.Deserialize(data, typeof(DotSightsSettings), DotSightSettingsGenerationContext.Default) as DotSightsSettings;
		}
		return new DotSightsSettings();
	}
	public static string SerializeData(List<ActivityData> data)
	{
		return JsonSerializer.Serialize(data, typeof(List<ActivityData>), ActivityDataListGenerationContext.Default);
	}

	public static bool DeserializeData(string data, out List<ActivityData>? result)
	{
		try
		{
			result = JsonSerializer.Deserialize(data, typeof(List<ActivityData>), ActivityDataListGenerationContext.Default) as List<ActivityData>;
			return true;
		}
		catch (Exception)
		{
			result = null;
			return false;
		}
	}

	public static List<ActivityData> GetDataFromDataPath()
	{
		if (File.Exists(DataFilePath))
		{
			string data = File.ReadAllText(DataFilePath);
			if (DeserializeData(data, out List<ActivityData>? result))
			{
				return result;
			}
		}
		return new List<ActivityData>();
	}

	public static void SaveDataToDataPath(List<ActivityData> data)
	{
		File.WriteAllText(DataFilePath, SerializeData(data));
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
		if (hourlyUsageInSeconds.Any(x => x >= 3600))
		{
			plotValues = hourlyUsageInSeconds.Select(x => x / 3600).ToArray();
		}
		else if (hourlyUsageInSeconds.Any(x => x >= 60))
		{
			plotValues = hourlyUsageInSeconds.Select(x => x / 60).ToArray();
		}
		else
		{
			plotValues = hourlyUsageInSeconds;
		}

		var barPlot = plot2.Add.Bars(Enumerable.Range(0, 24).Select(e => (double)e).ToArray(), plotValues);
		plot2.FigureBackground.Color = Color.FromHex("#00000000");
		plot2.Axes.Color(Color.FromHex("#FFFFFF"));
		plot2.Axes.Margins(0, 0, 0);

		ScottPlot.TickGenerators.NumericAutomatic gen = new() { LabelFormatter = (val) => DotFormatting.FormatHourToComputerClock((int)val) };

		plot2.Axes.Bottom.TickGenerator = gen;

		barPlot.ValueLabelStyle.ForeColor = Color.FromHex("#FFFFFF");
		barPlot.Color = Color.FromHex("#F68A06");

		var bars = barPlot.Bars.ToList();
		for (int i = 0; i < 24; i++)
		{
			bars[i].Label = DotFormatting.FormatTimeShort((int)hourlyUsageInSeconds[i]);
			bars[i].BorderLineWidth = 0;
		}


		plot2.SavePng("HourlyTimeUsagePlot.png", 1080, 540);

		Plot plot3 = new();

		double[] vals = [1, 2, 3, 4, 5];
		plot3.Add.Scatter(vals, vals);

		plot3.SaveBmp("ScatterPlot.bmp", 1920, 540);


	}
}

