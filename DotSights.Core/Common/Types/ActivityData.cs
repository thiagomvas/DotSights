using System.Text;

namespace DotSights.Core.Common.Types
{
	public class ActivityData
	{
		public string WindowTitle { get; set; }
		public long FocusedTimeInSeconds { get; set; }
		public Dictionary<DayOfWeek, int> UsageTimePerWeekDay { get; set; }
		public Dictionary<int, int> UsageTimePerHour { get; set; }
		public Dictionary<int, int> UsageTimePerMonth { get; set; }
		public string? Alias { get; set; }

		public ActivityData(string windowTitle)
		{
			WindowTitle = windowTitle;
			FocusedTimeInSeconds = 0;
			UsageTimePerWeekDay = new Dictionary<DayOfWeek, int>();
			UsageTimePerHour = new Dictionary<int, int>();
			UsageTimePerMonth = new Dictionary<int, int>();
		}
		public ActivityData()
		{
			WindowTitle = string.Empty;
			FocusedTimeInSeconds = 0;
			UsageTimePerWeekDay = new Dictionary<DayOfWeek, int>();
			UsageTimePerHour = new Dictionary<int, int>();
			UsageTimePerMonth = new Dictionary<int, int>();
		}
		public static ActivityData operator ++(ActivityData activityData)
		{
			activityData.FocusedTimeInSeconds++;

			DayOfWeek currentDay = DateTime.Now.DayOfWeek;
			if (activityData.UsageTimePerWeekDay.ContainsKey(currentDay))
			{
				activityData.UsageTimePerWeekDay[currentDay]++;
			}
			else
			{
				activityData.UsageTimePerWeekDay.Add(currentDay, 1);
			}

			int currentHour = DateTime.Now.Hour;
			if (activityData.UsageTimePerHour.ContainsKey(currentHour))
			{
				activityData.UsageTimePerHour[currentHour]++;
			}
			else
			{
				activityData.UsageTimePerHour.Add(currentHour, 1);
			}

			int currentMonth = DateTime.Now.Month;
			if (activityData.UsageTimePerMonth.ContainsKey(currentMonth))
			{
				activityData.UsageTimePerMonth[currentMonth]++;
			}
			else
			{
				activityData.UsageTimePerMonth.Add(currentMonth, 1);
			}
			return activityData;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"Window Title: {WindowTitle}");
			sb.AppendLine($"Focused Time: {FocusedTimeInSeconds} seconds");
			sb.AppendLine("Usage Time Per Week Day:");
			foreach (var kvp in UsageTimePerWeekDay)
			{
				sb.AppendLine($"{kvp.Key}: {kvp.Value}");
			}
			sb.AppendLine("Usage Time Per Hour:");
			foreach (var kvp in UsageTimePerHour)
			{
				sb.AppendLine($"{kvp.Key}: {kvp.Value}");
			}
			sb.AppendLine("Usage Time Per Month:");
			foreach (var kvp in UsageTimePerMonth)
			{
				sb.AppendLine($"{kvp.Key}: {kvp.Value}");
			}
			return sb.ToString();
		}
	}
}
