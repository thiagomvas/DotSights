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
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				var data = FilterDataFromSettings(GetDataFromDataPath(), LoadSettings());

				List<object[]> dataSet =
				[
					["Process Name", "Title", "Usage Time"],
					.. data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, DotFormatting.FormatTimeShort(d.TotalTimeToday) }),
				];

				Table t = Table.FromDataSet(dataSet);

				t.SetColumnColor(0, ConsoleColor.Yellow);

				t.Print();
			}
		}

		private class AllTimeCommand : Command
		{
			public AllTimeCommand() : base("alltime", "Display the total usage data tracked")
			{
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				var data = FilterDataFromSettings(GetDataFromDataPath(), LoadSettings());

				List<object[]> dataSet =
				[
					["Process Name", "Title", "Usage Time"],
					.. data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds) }),
				];

				Table t = Table.FromDataSet(dataSet);

				t.SetColumnColor(0, ConsoleColor.Yellow);

				t.Print();
			}
		}

		private class WeekCommand : Command
		{
			public WeekCommand() : base("week", "Display the total usage data tracked for the last 7 days")
			{
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				var data = FilterDataFromSettings(GetDataFromDataPath(), LoadSettings());

				List<object[]> dataSet =
				[
					["Process Name", "Title", "Usage Time"],
					.. data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek()) }),
				];

				Table t = Table.FromDataSet(dataSet);

				t.SetColumnColor(0, ConsoleColor.Yellow);

				t.Print();
			}
		}

		private class AllCommand : Command
		{
			public AllCommand() : base("all", "Display the total usage data tracked since the beginning, usage for today and usage for the last 7 days")
			{
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				var data = FilterDataFromSettings(GetDataFromDataPath(), LoadSettings());

				List<object[]> dataSet =
				[
					["Process Name", "Title", "All-Time Usage Time", "Today's Usage ", "This week's Usage"],
					.. data.Select(d => new object[] { d.ProcessName ?? string.Empty, d.WindowTitle, d.FormattedTotalUsageTime, DotFormatting.FormatTimeShort(d.TotalTimeToday), DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek()) })
				];

				Table t = Table.FromDataSet(dataSet);

				t.SetColumnColor(0, ConsoleColor.Yellow);

				t.Print();
			}
		}
	}
}
