namespace DotSights.Core.Common.Types
{
	public class GroupingRule
	{
		public string Name { get; set; }
		public string RegexQuery { get; set; }
		public bool ShowOnDashboard { get; set; } = true;
	}
}
