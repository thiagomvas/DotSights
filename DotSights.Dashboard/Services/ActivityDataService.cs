using System.Collections.Generic;
using System;
using DotSights.Core.Common.Types;
using System.IO;

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
