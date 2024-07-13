namespace DotSights.Core.Common.Types
{
	public class DotSightsSettings
	{
		#region Dashboard Settings
		public bool GroupItemsWithSameProcessName { get; set; } = true;
		public bool GroupItemsUsingGroupingRules { get; set; } = false;
		public List<GroupingRule> GroupingRules { get; set; } = new();
		public bool ShowOnlyRegexMatchedItems { get; set; } = false;
		public bool RegexMatchProcessName { get; set; } = false;
		#endregion

		#region Tracker Settings
		public TimeSpan TrackerSaveInterval { get; set; } = TimeSpan.FromMinutes(15);
		public TimeSpan DataBackupInterval { get; set; } = TimeSpan.FromDays(5);
		public int MaxBackupFileCount = 5;
		public bool OptimizeForStorageSpace { get; set; } = false;
		public List<string> GroupedProcessNames { get; set; } = new();

		public string TrackerExeLocation { get; set; } = string.Empty;
		#endregion
	}
}
