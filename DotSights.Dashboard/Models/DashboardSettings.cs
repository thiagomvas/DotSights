using DotSights.Core.Common.Types;
using System.Collections.Generic;

namespace DotSights.Dashboard.Models
{
	public class DashboardSettings
	{
		public bool GroupItemsWithSameProcessName { get; set; } = true;
		public bool GroupItemsUsingGroupingRules { get; set; } = false;
		public List<GroupingRule> GroupingRules { get; set; } = new();
	}
}
