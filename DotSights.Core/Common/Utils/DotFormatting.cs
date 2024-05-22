namespace DotSights.Core.Common.Utils
{
	public static class DotFormatting
	{
		public static string FormatTimeShort(int seconds)
		{
			if (seconds <= 0)
				return "0s";

			int hours = seconds / 3600;
			int minutes = (seconds % 3600) / 60;
			int secs = seconds % 60;

			string formattedTime = "";

			if (hours > 0)
				formattedTime += $"{hours}h ";

			if (minutes > 0)
				formattedTime += $"{minutes}m ";

			if (secs > 0)
				formattedTime += $"{secs}s";

			return formattedTime;

		}

		public static string FormatTimeLong(int seconds)
		{
			if (seconds <= 0)
				return "0 seconds";

			int hours = seconds / 3600;
			int minutes = (seconds % 3600) / 60;
			int secs = seconds % 60;

			string formattedTime = "";

			if (hours > 0)
				formattedTime += $"{hours} hours ";

			if (minutes > 0)
				formattedTime += $"{minutes} minutes ";

			if (secs > 0)
				formattedTime += $"{secs} seconds";

			return formattedTime;
		}

		public static string FormatHourToComputerClock(int hour)
		{
			TimeOnly time = new(hour, 0);

			return time.ToString();
		}
	}
}
