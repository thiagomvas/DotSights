using Avalonia;
using DotSights.Core;
using DotSights.Core.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotSights.Avalonia.Services
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
