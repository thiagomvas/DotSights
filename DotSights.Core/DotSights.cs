using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using ScottPlot;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotSights.Core;

public static class DotSights
{
    public static string AppDataPath { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DotSights");
    public static string DataFilePath { get; private set; } = Path.Combine(AppDataPath, "DotSights.Data.json");
    public static string DailyDataFilePath { get; private set; } = Path.Combine(AppDataPath, $"DotSights.Daily.json");
    public static string SettingsFilePath { get; private set; } = Path.Combine(AppDataPath, "DotSights.Settings.json");
    public static string TrackerFilePath { get; private set; } = Path.Combine(AppDataPath, "DotSights.Tracker.exe");

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
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(settings, typeof(DotSightsSettings), DotSightSettingsGenerationContext.Default));
    }

    public static DotSightsSettings LoadSettings()
    {
        if (File.Exists(SettingsFilePath))
        {
            string data = File.ReadAllText(SettingsFilePath);
            var result = JsonSerializer.Deserialize(data, typeof(DotSightsSettings), DotSightSettingsGenerationContext.Default) as DotSightsSettings;
            result!.GroupedProcessNames = result.GroupedProcessNames.Select(x => x.ToLower()).ToList();
            return result;
        }
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(new DotSightsSettings(), typeof(DotSightsSettings), DotSightSettingsGenerationContext.Default));
        return new DotSightsSettings();
    }
    public static string SerializeData(List<ActivityData> data)
    {
        return JsonSerializer.Serialize(data, typeof(List<ActivityData>), ActivityDataListGenerationContext.Default);
    }

    public static string SerializeData(List<DailyData> data)
    {
        return JsonSerializer.Serialize(data, typeof(List<DailyData>), DailyDataGenerationContext.Default);
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

    public static bool DeserializeData(string data, out List<DailyData>? result)
    {
        try
        {
            result = JsonSerializer.Deserialize(data, typeof(List<DailyData>), DailyDataGenerationContext.Default) as List<DailyData>;
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
                return result ?? new List<ActivityData>();
            }
        }
        return new List<ActivityData>();
    }
    public static List<DailyData> GetDailyDataFromDataPath()
    {
        if (File.Exists(DailyDataFilePath))
        {
            string data = File.ReadAllText(DailyDataFilePath);
            if (DeserializeData(data, out List<DailyData>? result))
            {
                return result ?? new List<DailyData>();
            }
        }
        return new List<DailyData>();
    }
    public static List<ActivityData> GetDataFromBackup(string fileName)
    {
        var path = Path.Combine(AppDataPath, fileName);

        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            if (DeserializeData(data, out List<ActivityData>? result))
            {
                return result ?? new List<ActivityData>();
            }
        }
        return new List<ActivityData>();
    }

    public static async Task<List<ActivityData>> GetDataFromDataPathAsync()
    {
        if (File.Exists(DataFilePath))
        {
            string data = await File.ReadAllTextAsync(DataFilePath);
            if (DeserializeData(data, out List<ActivityData>? result))
            {
                return result ?? new List<ActivityData>();
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
    }
    public static List<ActivityData> FilterDataFromRules(List<ActivityData> data, DotSightsSettings settings)
    {
        List<ActivityData> matches = new();
        List<ActivityData> matchedItems = new();

        foreach (var rule in settings.GroupingRules)
        {
            Regex regex = new(rule.RegexQuery, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            ActivityData? match = null;
            foreach (var item in data)
            {
                if (regex.IsMatch(item.WindowTitle))
                {
                    if (match == null)
                        match = new() { WindowTitle = rule.Name, ProcessName = item.ProcessName, Alias = item.Alias };
                    match += item;

                    if (!matchedItems.Contains(item))
                        matchedItems.Add(item);
                }
                else if (settings.RegexMatchProcessName && regex.IsMatch(item.ProcessName ?? string.Empty))
                {
                    if (match == null)
                        match = new() { WindowTitle = rule.Name, ProcessName = item.ProcessName, Alias = item.Alias };
                    match += item;

                    if (!matchedItems.Contains(item))
                        matchedItems.Add(item);
                }
            }
            if (match is not null) matches.Add(match);
        }

        // Remove items that have Display set to false
        matches = matches.Where(x => settings.GroupingRules.First(y => y.Name == x.WindowTitle).ShowOnDashboard).ToList();

        if (settings.ShowOnlyRegexMatchedItems)
            return matches;
        else return data.Except(matchedItems).Concat(matches).ToList();

    }
    public static List<ActivityData> FilterDataFromSettings(List<ActivityData> data, DotSightsSettings settings)
    {
        List<ActivityData> result = data.Select(x => x).ToList();

        // Group data with the same process name together
        if (settings.GroupItemsWithSameProcessName)
        {
            result = result.GroupBy(x => x.ProcessName).Select(x => new ActivityData
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

        if (settings.GroupItemsUsingGroupingRules && settings.GroupingRules.Count > 0)
            result = FilterDataFromRules(result, settings);

        return result;
    }

    public static void AssureSetup()
    {
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }
        if (!Directory.Exists(Path.GetDirectoryName(DataFilePath)))
        {
            SaveDataToDataPath(new List<ActivityData>());
        }
        if (!Directory.Exists(Path.GetDirectoryName(SettingsFilePath)))
        {
            SaveSettings(new DotSightsSettings());
        }
    }

    public static void SetTrackerToStartup()
    {
        File.Copy(TrackerFilePath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "DotSights.Tracker.exe"), true);
    }

    public static void RemoveTrackerFromStartup()
    {
        File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "DotSights.Tracker.exe"));
    }

    public static bool IsTrackerInStartup()
    {
        return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "DotSights.Tracker.exe"));
    }

    public static void CopyTrackerToAppdata(string path)
    {
        File.Copy(path, TrackerFilePath, true);
    }

    public static void CreateDataChartForActivity(ActivityData data)
    {
        GenerateActiveHoursChart(data);

        // Active day of the week
        GenerateActiveDaysChart(data);
    }

    private static void GenerateActiveDaysChart(ActivityData data)
    {
        Plot plot2 = new();

        plot2.Title("Most Active Days", 20);

        var dayUsageInSeconds = Enumerable.Range(0, 7).Select(day => data.UsageTimePerWeekDay.ContainsKey((DayOfWeek)day) ? (double)data.UsageTimePerWeekDay[(DayOfWeek)day] : 0).ToArray();

        double[] plotValues2 = new double[7];

        if (dayUsageInSeconds.Any(x => x >= 3600))
        {
            plotValues2 = dayUsageInSeconds.Select(x => x / 3600).ToArray();
        }
        else if (dayUsageInSeconds.Any(x => x >= 60))
        {
            plotValues2 = dayUsageInSeconds.Select(x => x / 60).ToArray();
        }
        else
        {
            plotValues2 = dayUsageInSeconds;
        }

        var barPlot2 = plot2.Add.Bars(Enumerable.Range(0, 7).Select(e => (double)e).ToArray(), plotValues2);

        plot2.FigureBackground.Color = Color.FromHex("#00000000");

        plot2.Axes.Color(Color.FromHex("#FFFFFF"));

        plot2.Axes.Margins(0, 0, 0);

        ScottPlot.TickGenerators.NumericAutomatic gen2 = new() { LabelFormatter = (val) => ((DayOfWeek)(int)val).ToString().Substring(0, 3) };

        plot2.Axes.Bottom.TickGenerator = gen2;

        barPlot2.ValueLabelStyle.ForeColor = Color.FromHex("#FFFFFF");
        barPlot2.Color = Color.FromHex("#F68A06");

        var bars2 = barPlot2.Bars.ToList();
        for (int i = 0; i < 7; i++)
        {
            bars2[i].Label = DotFormatting.FormatTimeShort((int)dayUsageInSeconds[i]);
            bars2[i].BorderLineWidth = 0;
        }

        plot2.SavePng("ActiveDays.png", 1080, 540);
    }

    private static void GenerateActiveHoursChart(ActivityData data)
    {
        Plot plot1 = new();

        plot1.Title("Most Active Hours", 20);
        // Get hourly usage and expand to a 24-length array representing each hour
        var hourlyUsageInSeconds = Enumerable.Range(0, 24).Select(hour => data.UsageTimePerHour.ContainsKey(hour) ? (double)data.UsageTimePerHour[hour] : 0).ToArray();

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

        var barPlot = plot1.Add.Bars(Enumerable.Range(0, 24).Select(e => (double)e).ToArray(), plotValues);

        plot1.FigureBackground.Color = Color.FromHex("#00000000");
        plot1.Axes.Color(Color.FromHex("#FFFFFF"));
        plot1.Axes.Margins(0, 0, 0);

        ScottPlot.TickGenerators.NumericAutomatic gen = new() { LabelFormatter = (val) => DotFormatting.FormatHourToComputerClock((int)val) };

        plot1.Axes.Bottom.TickGenerator = gen;

        barPlot.ValueLabelStyle.ForeColor = Color.FromHex("#FFFFFF");
        barPlot.Color = Color.FromHex("#F68A06");

        var bars = barPlot.Bars.ToList();
        for (int i = 0; i < 24; i++)
        {
            bars[i].Label = DotFormatting.FormatTimeShort((int)hourlyUsageInSeconds[i]);
            bars[i].BorderLineWidth = 0;
        }

        plot1.SavePng("ActiveHours.png", 1080, 540);
    }

    /// <summary>
    /// Merges data that matches a rule into a single item
    /// </summary>
    /// <param name="data">The data</param>
    /// <param name="rule">The rule used to squash</param>
    /// <param name="useProcessName">Whether to match processes names as well</param>
    /// <returns>The data list with all matching entries merged together</returns>
    public static List<ActivityData> SquashDataUsingRule(List<ActivityData> data, GroupingRule rule, bool useProcessName)
    {
        Regex regex = new(rule.RegexQuery, RegexOptions.IgnoreCase);
        ActivityData? match = null;
        List<ActivityData> matchedItems = new();

        foreach (var item in data)
        {
            if (regex.IsMatch(item.WindowTitle))
            {
                if (match == null)
                    match = new() { WindowTitle = rule.Name, ProcessName = item.ProcessName, Alias = item.Alias };
                match += item;

                if (!matchedItems.Contains(item))
                    matchedItems.Add(item);
            }
            else if (useProcessName && regex.IsMatch(item.ProcessName ?? string.Empty))
            {
                if (match == null)
                    match = new() { WindowTitle = rule.Name, ProcessName = item.ProcessName, Alias = item.Alias };
                match += item;

                if (!matchedItems.Contains(item))
                    matchedItems.Add(item);
            }
        }

        if (match is not null) return data.Except(matchedItems).Append(match).ToList();
        else return data;
    }

    public static DateTime GetLastBackupDate()
    {
        // Check for backup files
        var files = Directory.GetFiles(AppDataPath, "*.backup", SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
        {
            var lastBackup = files.Select(x => new FileInfo(x)).OrderByDescending(x => x.CreationTime).First();
            return lastBackup.CreationTime;
        }
        return DateTime.MinValue;
    }

    public static void CreateNewDataBackup()
    {
        string backupPath = Path.Combine(AppDataPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.backup");
        File.Copy(DataFilePath, backupPath);
    }

    public static void AssureBackupFileCount(int maxFileCount)
    {
        var files = Directory.GetFiles(AppDataPath, "*.backup", SearchOption.TopDirectoryOnly);
        if (files.Length > maxFileCount)
        {
            var toDelete = files.Select(x => new FileInfo(x)).OrderBy(x => x.CreationTime).Take(files.Length - maxFileCount);
            foreach (var file in toDelete)
            {
                file.Delete();
            }
        }
    }

    public static void SetDataPath(string dataPath, string dailyDataPath)
    {
        DataFilePath = Path.Combine(AppDataPath, dataPath);
        DailyDataFilePath = Path.Combine(AppDataPath, dailyDataPath);
    }
}

