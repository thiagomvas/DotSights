using System.Text;
using System.Text.RegularExpressions;

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
		public string? ProcessName { get; set; }
		public Dictionary<DateTime, int> Last7DaysUsage { get; set; }

		public int TotalTimeToday => Last7DaysUsage.ContainsKey(DateTime.Today) ? Last7DaysUsage[DateTime.Today] : 0;
		public string FormattedTotalUsageTime
		{
			get
			{
				if(FocusedTimeInSeconds > 3600)
				{
					return $"{FocusedTimeInSeconds / 3600}h {FocusedTimeInSeconds % 3600 / 60}m {FocusedTimeInSeconds % 60}s";
				}
				else if(FocusedTimeInSeconds > 60)
				{
					return $"{FocusedTimeInSeconds / 60}m {FocusedTimeInSeconds % 60}s";
				}
				else
				{
					return $"{FocusedTimeInSeconds}s";
				}
			}
		}
		public ActivityData(string windowTitle)
		{
			WindowTitle = windowTitle;
			FocusedTimeInSeconds = 0;
			UsageTimePerWeekDay = new Dictionary<DayOfWeek, int>();
			UsageTimePerHour = new Dictionary<int, int>();
			UsageTimePerMonth = new Dictionary<int, int>();
			Last7DaysUsage = new Dictionary<DateTime, int>();
		}
		public ActivityData()
		{
			WindowTitle = string.Empty;
			FocusedTimeInSeconds = 0;
			UsageTimePerWeekDay = new Dictionary<DayOfWeek, int>();
			UsageTimePerHour = new Dictionary<int, int>();
			UsageTimePerMonth = new Dictionary<int, int>();
			Last7DaysUsage = new Dictionary<DateTime, int>();
		}

		public static ActivityData operator +(ActivityData left, ActivityData right)
		{
			ActivityData result = new ActivityData();

			result.WindowTitle = left.WindowTitle;
			result.FocusedTimeInSeconds = left.FocusedTimeInSeconds + right.FocusedTimeInSeconds;

			result.UsageTimePerWeekDay = new Dictionary<DayOfWeek, int>();
			foreach (var kvp in left.UsageTimePerWeekDay)
			{
				result.UsageTimePerWeekDay[kvp.Key] = kvp.Value;
			}
			foreach (var kvp in right.UsageTimePerWeekDay)
			{
				if (result.UsageTimePerWeekDay.ContainsKey(kvp.Key))
				{
					result.UsageTimePerWeekDay[kvp.Key] += kvp.Value;
				}
				else
				{
					result.UsageTimePerWeekDay[kvp.Key] = kvp.Value;
				}
			}

			result.UsageTimePerHour = new Dictionary<int, int>();
			foreach (var kvp in left.UsageTimePerHour)
			{
				result.UsageTimePerHour[kvp.Key] = kvp.Value;
			}
			foreach (var kvp in right.UsageTimePerHour)
			{
				if (result.UsageTimePerHour.ContainsKey(kvp.Key))
				{
					result.UsageTimePerHour[kvp.Key] += kvp.Value;
				}
				else
				{
					result.UsageTimePerHour[kvp.Key] = kvp.Value;
				}
			}

			result.UsageTimePerMonth = new Dictionary<int, int>();
			foreach (var kvp in left.UsageTimePerMonth)
			{
				result.UsageTimePerMonth[kvp.Key] = kvp.Value;
			}
			foreach (var kvp in right.UsageTimePerMonth)
			{
				if (result.UsageTimePerMonth.ContainsKey(kvp.Key))
				{
					result.UsageTimePerMonth[kvp.Key] += kvp.Value;
				}
				else
				{
					result.UsageTimePerMonth[kvp.Key] = kvp.Value;
				}
			}

			result.Alias = left.Alias;
			result.ProcessName = left.ProcessName;

			result.Last7DaysUsage = new Dictionary<DateTime, int>();
			foreach (var kvp in left.Last7DaysUsage)
			{
				result.Last7DaysUsage[kvp.Key] = kvp.Value;
			}
			foreach (var kvp in right.Last7DaysUsage)
			{
				if (result.Last7DaysUsage.ContainsKey(kvp.Key))
				{
					result.Last7DaysUsage[kvp.Key] += kvp.Value;
				}
				else
				{
					result.Last7DaysUsage[kvp.Key] = kvp.Value;
				}
			}

			return result;
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

			activityData.EnsureLast7DaysData();

			DateTime today = DateTime.Today;
			activityData.Last7DaysUsage[today] = activityData.Last7DaysUsage.GetValueOrDefault(today) + 1;

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

		public void EnsureLast7DaysData()
		{
			DateTime today = DateTime.Today;
			DateTime startDate = today.AddDays(-6); 
			for (DateTime date = startDate; date < today; date = date.AddDays(1))
			{
				if (!Last7DaysUsage.ContainsKey(date))
				{
					Last7DaysUsage.Add(date, 0);
				}
			}

			if (!Last7DaysUsage.ContainsKey(today))
			{
				Last7DaysUsage.Add(today, 0);
			}

			Last7DaysUsage = Last7DaysUsage
				.Where(kv => (today - kv.Key).TotalDays <= 6)
				.ToDictionary(kv => kv.Key, kv => kv.Value);
		}


		public static List<ActivityData> GroupByRule(IEnumerable<ActivityData> activityDataList, GroupingRule rule, bool includeUnmatched = true, bool matchProcessNames = false)
		{
			List<ActivityData> groupedDataList = new List<ActivityData>();
			List<ActivityData> unmatchedDataList = new List<ActivityData>();
			string regexPattern = rule.RegexQuery;

			// Regex to match the provided pattern
			Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

			// Initialize a variable to hold the merged data
			ActivityData mergedData = null;

			foreach (ActivityData data in activityDataList)
			{
				if (regex.IsMatch(data.WindowTitle))
				{
					// If this is the first matching data, initialize mergedData
					if (mergedData == null)
					{
						mergedData = data;
						mergedData.WindowTitle = rule.Name;
					}
					else
					{
						// Merge the current data with the existing mergedData
						mergedData += data;
					}
				}
				else if(matchProcessNames && regex.IsMatch(data.ProcessName))
				{
					// If this is the first matching data, initialize mergedData
					if (mergedData == null)
					{
						mergedData = data;
						mergedData.WindowTitle = rule.Name;
					}
					else
					{
						// Merge the current data with the existing mergedData
						mergedData += data;
					}

				}
				else
				{
					// Add unmatched data to the unmatched list
					unmatchedDataList.Add(data);
				}
			}

			// If mergedData is not null, add it to the groupedDataList
			if (mergedData != null)
			{
				groupedDataList.Add(mergedData);
			}

			// Add the unmatched data to the groupedDataList
			if(includeUnmatched) 
				groupedDataList.AddRange(unmatchedDataList);

			return groupedDataList;
		}
	}
}

