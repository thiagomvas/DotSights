using DotSights.Core;
using DotSights.Core.Common.Types;
using System.Collections.Generic;

namespace DotSights.Avalonia.Services
{
	public class ActivityDataService
	{
		public IEnumerable<ActivityData> GetActivityData() => [
			new() {WindowTitle = "Discord", FocusedTimeInSeconds = 20, ProcessName = "discord"},
			new() {WindowTitle = "Visual Studio Code", FocusedTimeInSeconds = 30, ProcessName = "code"},
			new() {WindowTitle = "Google Chrome", FocusedTimeInSeconds = 40, ProcessName = "chrome"},
		];
	}
}
