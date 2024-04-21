
namespace DotSights.ConsoleApp;

using DotSights.Core;
using DotSights.Core.Common;

class Program
{
	public static void Main(string[] args)
	{
		DotSights.DataFilePath = @"C:\Users\Thiago\source\repos\DotSights\DotSights.Tracker\DotSightsData.json";
		var result = DotSights.GetDataFromDataPath();

		var hourlyTotalTime = Enumerable.Range(0, 24).Select(hour => result.Sum(e => e.UsageTimePerHour.ContainsKey(hour) ? e.UsageTimePerHour[hour] : 0 )).ToArray();

		Console.WriteLine("Total time spent on each hour of the day:");
		for (int i = 0; i < hourlyTotalTime.Length; i++)
		{
			Console.WriteLine($"{i}: {hourlyTotalTime[i]}");
		}
	}
}

