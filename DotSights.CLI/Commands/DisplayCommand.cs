using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using SharpTables;
using SharpTables.Graph;
using System.Collections.Specialized;
using System.CommandLine;
using System.Globalization;
using static DotSights.Core.DotSights;

namespace DotSights.CLI.Commands
{
    internal class DisplayCommand : BaseCommand
    {
        public static readonly Action<Cell> template = c =>
        {
            if (c.Text.Length > 25)
                c.Text = c.Text.Substring(0, 25) + "...";

            if (c.Position.X == 0)
                c.Color = ConsoleColor.Gray;

            if (c.Position.X == 2)
                c.Color = ConsoleColor.Yellow;
        };
        public DisplayCommand() : base("display", "Displays data tracked by DotSights directly on the terminal")
        {
        }

        public override void Setup(RootCommand root)
        {
            AddCommand(new TodayCommand());
            AddCommand(new AllTimeCommand());
            AddCommand(new WeekCommand());
            AddCommand(new AllCommand());
            AddCommand(new OverallCommand());

            root.Add(this);
        }

        public static List<ActivityData> SetupData(bool showAll, bool orderAlphabetical, bool orderTime)
        {
            var data = GetDataFromDataPath();

            if (!showAll)
                data = FilterDataFromSettings(GetDataFromDataPath(), LoadSettings());

            if (orderAlphabetical)
                data = data.OrderBy(d => d.WindowTitle).ToList();

            return data;
        }

        public static void ApplyTableTemplate(Table t)
        {
            t.UsePreset(template)
                .UseNullOrEmptyReplacement("N/A");
        }

        private class TodayCommand : Command
        {
            public TodayCommand() : base("today", "Display the total usage data tracked for today")
            {
                var optionShowAll = new Option<bool>(new[] { "--showall", "-s" }, "Ignore Regex Grouping rules (if any) and show all the data tracked");
                var optionOrderAlphabetical = new Option<bool>(new[] { "--orderalphabetical", "-a" }, "Order data alphabetically");
                var optionOrderTime = new Option<bool>(new[] { "--ordertime", "-t" }, "Order data by time");
                var optionDateOffset = new Option<int>(new[] { "--dateoffset", "-d" }, "Display the daily data for 'n' days ago. 0 is today, 6 is 6 days ago (max). Does not show activities for previous days");

                this.AddOption(optionShowAll);
                this.AddOption(optionOrderAlphabetical);
                this.AddOption(optionOrderTime);
                this.AddOption(optionDateOffset);

                this.SetHandler(Execute, optionShowAll, optionOrderAlphabetical, optionOrderTime, optionDateOffset);
            }

            private void Execute(bool showAll, bool orderAlphabetical, bool orderTime, int dateOffset)
            {
                var data = SetupData(showAll, orderAlphabetical, orderTime);
                data = data.Where(d => d.TotalTimeToday > 0).ToList();
                if (orderTime)
                    data = data.OrderByDescending(d => d.TotalTimeToday).ToList();

                Table t = Table.FromDataSet(data, d =>
                {
                    var processName = d.ProcessName ?? string.Empty;
                    var title = d.WindowTitle;
                    var time = DotFormatting.FormatTimeShort(d.TotalTimeToday);
                    return new Row(processName, title, time);
                });

                t.SetHeader(new("Process name", "Title", "Usage time"));

                ApplyTableTemplate(t);
                if(dateOffset == 0)
                    t.Write();

                var dailyhourly = GetDailyDataFromDataPath().OrderByDescending(d => d.Date).ToArray()[dateOffset].UsageTimePerHour;
                var max = dailyhourly.Max(kvp => kvp.Value);
                var min = dailyhourly.Min(kvp => kvp.Value);
                for(int i = 0; i < DateTime.Now.Hour; i++)
                {
                    if (!dailyhourly.ContainsKey(i))
                        dailyhourly.Add(i, 0);
                }
                dailyhourly = dailyhourly.OrderBy(KeyValuePair => KeyValuePair.Key).ToDictionary();
                if(DateTime.Now.Hour >= 12)
                {
                    var graph1 = new Graph<KeyValuePair<int, int>>(dailyhourly.Where(kvp => kvp.Key < 12))
                        .UseValueGetter(kv => kv.Value)
                        .UseXTickFormatter(kv => kv.Key.ToString("0"))
                        .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                        .UseMinValue(min)
                        .UseMaxValue(max)
                        .UseHeader("Hourly Usage (Day)")
                        .UseYAxisPadding(1);
                    graph1.Write();
                    Console.WriteLine();
                    var graph2 = new Graph<KeyValuePair<int, int>>(dailyhourly.Where(kvp => kvp.Key >= 12))
                        .UseValueGetter(kv => kv.Value)
                        .UseXTickFormatter(kv => kv.Key.ToString("0"))
                        .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                        .UseHeader("Hourly Usage (Night)")
                        .UseMinValue(min)
                        .UseMaxValue(max)
                        .UseYAxisPadding(1);
                    graph2.Write();
                }
                else
                {
                    var graph = new Graph<KeyValuePair<int, int>>(dailyhourly.Take(12))
                        .UseValueGetter(kv => kv.Value)
                        .UseXTickFormatter(kv => kv.Key.ToString("0"))
                        .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                        .UseMinValue(min)
                        .UseMaxValue(max)
                        .UseHeader("Hourly Usage")
                        .UseYAxisPadding(1);
                    graph.Write();
                }
            }
        }
        private class AllTimeCommand : Command
        {
            public AllTimeCommand() : base("alltime", "Display the total usage data tracked")
            {
                var optionShowAll = new Option<bool>(new[] { "--showall", "-s" }, "Ignore Regex Grouping rules (if any) and show all the data tracked");
                var optionOrderAlphabetical = new Option<bool>(new[] { "--orderalphabetical", "-a" }, "Order data alphabetically");
                var optionOrderTime = new Option<bool>(new[] { "--ordertime", "-t" }, "Order data by time");

                this.AddOption(optionShowAll);
                this.AddOption(optionOrderAlphabetical);
                this.AddOption(optionOrderTime);

                this.SetHandler(Execute, optionShowAll, optionOrderAlphabetical, optionOrderTime);
            }

            private void Execute(bool showAll, bool orderAlphabetical, bool orderTime)
            {
                var data = SetupData(showAll, orderAlphabetical, orderTime);

                if (orderTime)
                    data = data.OrderByDescending(d => d.FocusedTimeInSeconds).ToList();

                Table t = Table.FromDataSet(data, d =>
                {
                    var processName = d.ProcessName ?? string.Empty;
                    var title = d.WindowTitle;
                    var alltime = DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds);
                    return new Row(processName, title, alltime);
                });

                t.SetHeader(new("Process Name", "Title", "Total usage"));


                ApplyTableTemplate(t);

                Utils.EnterInteractableMode(t);
            }
        }

        private class WeekCommand : Command
        {
            public WeekCommand() : base("week", "Display the total usage data tracked for the last 7 days")
            {
                var optionShowAll = new Option<bool>(new[] { "--showall", "-s" }, "Ignore Regex Grouping rules (if any) and show all the data tracked");
                var optionOrderAlphabetical = new Option<bool>(new[] { "--orderalphabetical", "-a" }, "Order data alphabetically");
                var optionOrderTime = new Option<bool>(new[] { "--ordertime", "-t" }, "Order data by time");

                this.AddOption(optionShowAll);
                this.AddOption(optionOrderAlphabetical);
                this.AddOption(optionOrderTime);

                this.SetHandler(Execute, optionShowAll, optionOrderAlphabetical, optionOrderTime);
            }

            private void Execute(bool showAll, bool orderAlphabetical, bool orderTime)
            {
                var data = SetupData(showAll, orderAlphabetical, orderTime);
                data = data.Where(d => d.GetUsageTimeForWeek() > 0).ToList();
                if (orderTime)
                    data = data.OrderByDescending(d => d.GetUsageTimeForWeek()).ToList();

                Table t = Table.FromDataSet(data, d =>
                {
                    var processName = d.ProcessName ?? string.Empty;
                    var title = d.WindowTitle;
                    var time = DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek());
                    return new Row(processName, title, time);
                });

                t.SetHeader(new("Process name", "Title", "Usage time"));

                ApplyTableTemplate(t);

                t.Write();

                Dictionary<DayOfWeek, int> usagePerDayOfWeek = new();
                // Fill with current week data. If data is missing, fill with 0
                DateTime today = DateTime.Today;
                DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                DateTime endOfWeek = startOfWeek.AddDays(7);
                for (DateTime date = startOfWeek; date < endOfWeek; date = date.AddDays(1))
                {
                    usagePerDayOfWeek[date.DayOfWeek] = data.Sum(d => d.Last7DaysUsage.GetValueOrDefault(date, 0));
                }

                var graph = new Graph<KeyValuePair<DayOfWeek, int>>(usagePerDayOfWeek.OrderBy(kv => kv.Key))
                    .UseValueGetter(kv => kv.Value)
                    .UseXTickFormatter(kv => DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(kv.Key))
                    .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                    .UseHeader("Usage per day of week")
                    .UseYAxisPadding(1);

                graph.Write();
            }
        }

        private class AllCommand : Command
        {
            public AllCommand() : base("all", "Display the total usage data tracked since the beginning, usage for today and usage for the last 7 days")
            {
                var optionShowAll = new Option<bool>(new[] { "--showall", "-s" }, "Ignore Regex Grouping rules (if any) and show all the data tracked");
                var optionOrderAlphabetical = new Option<bool>(new[] { "--orderalphabetical", "-a" }, "Order data alphabetically");
                var optionOrderTime = new Option<bool>(new[] { "--ordertime", "-t" }, "Order data by time");

                this.AddOption(optionShowAll);
                this.AddOption(optionOrderAlphabetical);
                this.AddOption(optionOrderTime);

                this.SetHandler(Execute, optionShowAll, optionOrderAlphabetical, optionOrderTime);
            }

            private void Execute(bool showAll, bool orderAlphabetical, bool orderTime)
            {
                var data = SetupData(showAll, orderAlphabetical, orderTime);
                if (orderTime)
                    data = data.OrderByDescending(d => d.FocusedTimeInSeconds).ToList();

                Table t = Table.FromDataSet(data, d =>
                {
                    var processName = d.ProcessName ?? string.Empty;
                    var title = d.WindowTitle;
                    var alltime = DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds);
                    var todaytime = DotFormatting.FormatTimeShort(d.TotalTimeToday);
                    var weektime = DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek());
                    return new Row(processName, title, alltime, todaytime, weektime);
                });

                t.SetHeader(new("Process Name", "Title", "Total usage", "Today's usage", "Past 7 days' usage"));

                ApplyTableTemplate(t);

                Utils.EnterInteractableMode(t);
            }
        }

        private class OverallCommand : Command
        {
            public OverallCommand() : base("overall", "Displays overall data such as most used process, most active day of week and hours")
            {
                var showInfoOption = new Option<bool?>(new[] { "--showinfo", "-i" }, "Show info such as most used process, most active day of week and hours");
                var showGraphsOption = new Option<bool?>(new[] { "--showgraphs", "-g" }, "Show graphs for usage per day of week and hour of day");

                this.AddOption(showInfoOption);
                this.AddOption(showGraphsOption);

                this.SetHandler(Execute, showInfoOption, showGraphsOption);
            }

            public void Execute(bool? showInfo, bool? showGraphs)
            {
                if (showInfo == null)
                    showInfo = true;

                var data = GetDataFromDataPath();
                ActivityData total = data.Aggregate((acc, d) => acc + d);
                bool is24Hour = DateTimeFormatInfo.CurrentInfo.ShortTimePattern.Contains("H");
                var timePattern = is24Hour ? "HH" : "h tt";
                if (showInfo == true)
                {

                    Console.WriteLine("Most used process: " + data.MaxBy(d => d.FocusedTimeInSeconds).ProcessName);
                    Console.WriteLine("Most active day of week: " + DateTimeFormatInfo.CurrentInfo.GetDayName(total.UsageTimePerWeekDay.MaxBy(kv => kv.Value).Key));
                    Console.WriteLine("Most active hour of day: " + DateTime.Today.AddHours(total.UsageTimePerHour.MaxBy(kv => kv.Value).Key).ToString(timePattern));
                    Console.WriteLine("Most active month: " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(total.UsageTimePerMonth.MaxBy(kv => kv.Value).Key));
                    Console.WriteLine();

                }

                if(showGraphs == true)
                {

                    Graph<KeyValuePair<DayOfWeek, int>> dowGraph = new Graph<KeyValuePair<DayOfWeek, int>>(total.UsageTimePerWeekDay.OrderBy(kv => kv.Key))
                        .UseValueGetter(kv => kv.Value)
                        .UseXTickFormatter(kv => DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(kv.Key))
                        .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                        .UseHeader("Usage per day of week")
                        .UseYAxisPadding(1);
                    dowGraph.Write();
                    Console.WriteLine();
                    var hourlyUsage = total.UsageTimePerHour;
                    if (hourlyUsage.Count > 12 && hourlyUsage.Count < 24)
                    {
                        hourlyUsage = hourlyUsage.Concat(Enumerable.Range(0, 24).Except(hourlyUsage.Select(kv => kv.Key)).Select(h => new KeyValuePair<int, int>(h, 0))).OrderBy(kv => kv.Key).ToDictionary();
                    }

                    var todGraph = new Graph<KeyValuePair<int, int>>(hourlyUsage)
                        .UseValueGetter(kv => kv.Value)
                        .UseXTickFormatter(kv => DateTime.Today.AddHours(kv.Key).ToString(timePattern))
                        .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                        .UseMaxValue(total.UsageTimePerHour.MaxBy(kvp => kvp.Value).Value * 1.1)
                        .UseMinValue(total.UsageTimePerHour.MinBy(kvp => kvp.Value).Value)
                        .UseHeader("Usage per hour of day")
                        .UseYAxisPadding(1);

                    if (hourlyUsage.Count > 12)
                    {
                        foreach (var page in todGraph.ToPaginatedGraph(12))
                        {
                            page.Write();
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        todGraph.Write();
                    }

                    var monthlyGraph = new Graph<KeyValuePair<int, int>>(total.UsageTimePerMonth)
                        .UseValueGetter(kv => kv.Value)
                        .UseXTickFormatter(kv => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(kv.Key))
                        .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
                        .UseHeader("Usage per month")
                        .UseYAxisPadding(1);

                    monthlyGraph.Write();
                    Console.WriteLine();
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Lack of data for some hours, days or months means either no activity was tracked or the data is not available.");
                    Console.ResetColor();
                }
                

            }
        }
    }
}
