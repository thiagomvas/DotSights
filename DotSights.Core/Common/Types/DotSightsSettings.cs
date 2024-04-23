using DotSights.Core.Common.Types;
using System;
using System.Collections.Generic;

namespace DotSights.Dashboard.Models
{
	public class DotSightsSettings
	{
		#region Dashboard Settings
		public bool GroupItemsWithSameProcessName { get; set; } = true;
		public bool GroupItemsUsingGroupingRules { get; set; } = false;
		public List<GroupingRule> GroupingRules { get; set; } = new();
		#endregion

		#region Tracker Settings
		public TimeSpan TrackerSaveInterval { get; set; } = TimeSpan.FromMinutes(15);
		public bool OptimizeForStorageSpace { get; set; } = false;
		#endregion
	}
}
