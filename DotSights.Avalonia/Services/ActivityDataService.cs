using DotSights.Core;
using DotSights.Core.Common.Types;
using System.Collections.Generic;

namespace DotSights.Avalonia.Services
{
	public class ActivityDataService
	{
		public IEnumerable<ActivityData> GetActivityData()
		{
			Core.DotSights.DataFilePath = @"C:\Users\Thiago\source\repos\DotSights\DotSights.Tracker\DotSightsData.json";
			return Core.DotSights.GetDataFromDataPath();
		}
	}
}
