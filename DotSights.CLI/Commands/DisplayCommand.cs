using DotSights.Core;
using DotSights.Core.Common.Utils;
using SharpTables;
using System.CommandLine;
using System.CommandLine.Invocation;
using static DotSights.Core.DotSights;

namespace DotSights.CLI.Commands
{
	internal class DisplayCommand : BaseCommand
	{
		public static readonly Action<Cell> template = c =>
		{
			if (c.Position.X == 0)
				c.Color = ConsoleColor.Yellow;
			else
			{
				if (c.Text.Length > 50)
					c.Text = c.Text.Substring(0, 50) + "...";
			}
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
				var data = GetDataFromDataPath();

				if (!showAll)
					data = FilterDataFromSettings(GetDataFromDataPath(), LoadSettings());

				if (orderAlphabetical)
					data = data.OrderBy(d => d.WindowTitle).ToList();
				else if (orderTime)
					data = data.OrderByDescending(d => d.TotalTimeToday).ToList();

				List<object[]> dataSet =
				[
					["Process Name", "Title", "Usage Time"],
					.. data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, DotFormatting.FormatTimeShort(d.TotalTimeToday) }),
				];

				Table t = Table.FromDataSet(dataSet);

				t.UsePreset(template);

				t.Print();
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
				var data = GetDataFromDataPath();
				if (!showAll)
					data = FilterDataFromSettings(data, LoadSettings());

				if (orderAlphabetical)
					data = data.OrderBy(d => d.WindowTitle).ToList();
				else if (orderTime)
					data = data.OrderByDescending(d => d.FocusedTimeInSeconds).ToList();

				List<object[]> dataSet = new List<object[]>
		{
			new object[] { "Process Name", "Title", "Usage Time" }
		};

				dataSet.AddRange(data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds) }));

				Table t = Table.FromDataSet(dataSet);

				t.UsePreset(template);

				t.Print();
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
				var data = GetDataFromDataPath();
				if (!showAll)
					data = FilterDataFromSettings(data, LoadSettings());

				if (orderAlphabetical)
					data = data.OrderBy(d => d.WindowTitle).ToList();
				else if (orderTime)
					data = data.OrderByDescending(d => d.GetUsageTimeForWeek()).ToList();

				List<object[]> dataSet = new List<object[]>
		{
			new object[] { "Process Name", "Title", "Usage Time" }
		};

				dataSet.AddRange(data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek()) }));

				Table t = Table.FromDataSet(dataSet);

				t.UsePreset(template);

				t.Print();
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
				var data = GetDataFromDataPath();
				if (!showAll)
					data = FilterDataFromSettings(data, LoadSettings());

				if (orderAlphabetical)
					data = data.OrderBy(d => d.WindowTitle).ToList();
				else if (orderTime)
					data = data.OrderByDescending(d => d.TotalTimeToday).ToList();

				List<object[]> dataSet = new List<object[]>
		{
			new object[] { "Process Name", "Title", "All-Time Usage Time", "Today's Usage ", "This week's Usage" }
		};

				dataSet.AddRange(data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, d.FormattedTotalUsageTime, DotFormatting.FormatTimeShort(d.TotalTimeToday), DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek()) }));

				Table t = Table.FromDataSet(dataSet);

				t.UsePreset(template);

				t.Print();
			}
		}
	}
}
