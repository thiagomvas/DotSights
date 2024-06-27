namespace DotSights.Core.Common.Types
{
	public class GroupingRule
	{
		public string Name { get; set; } = string.Empty;
		public string RegexQuery { get; set; } = string.Empty;
		public bool ShowOnDashboard { get; set; } = true;
	}
}
