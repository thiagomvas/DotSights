using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using SharpTables;
using System.CommandLine;
using static DotSights.Core.DotSights;

namespace DotSights.CLI.Commands
{
    internal class DisplayCommand : BaseCommand
    {
        public static readonly Action<Cell> template = c =>
        {
            if (c.Text.Length > 25)
                c.Text = c.Text.Substring(0, 25) + "...";

            if(c.Position.X == 0)
                c.Color = ConsoleColor.Gray;

            if(c.Position.X == 2)
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

                this.AddOption(optionShowAll);
                this.AddOption(optionOrderAlphabetical);
                this.AddOption(optionOrderTime);

                this.SetHandler(Execute, optionShowAll, optionOrderAlphabetical, optionOrderTime);
            }

            private void Execute(bool showAll, bool orderAlphabetical, bool orderTime)
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

                Utils.EnterInteractableMode(t);
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

                Utils.EnterInteractableMode(t);
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
    }
}
