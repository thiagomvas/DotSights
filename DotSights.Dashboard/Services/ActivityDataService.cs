using DotSights.Core.Common.Types;
using System.Collections.Generic;

namespace DotSights.Dashboard.Services
{
	public class ActivityDataService
	{
		public IEnumerable<ActivityData> GetActivityData()
		{
			return Core.DotSights.GetDataFromDataPath();
		}
	}
}
