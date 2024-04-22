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
			Core.DotSights.DataFilePath = Path.Combine(Environment.CurrentDirectory, "DotSightsData.json");
			return Core.DotSights.GetDataFromDataPath();
		}
	}
}
