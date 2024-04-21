namespace DotSights.Core.Common.Utils
{
	public static class DotFormatting
	{
		public static string FormatTime(int seconds)
		{
			if (seconds <= 0)
				return "";

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
	}
}
