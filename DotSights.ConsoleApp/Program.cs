
namespace DotSights.ConsoleApp;

using DotSights.Core;
using DotSights.Core.Common;
using DotSights.Core.Common.Types;

class Program
{
	public static void Main(string[] args)
	{
		DotSights.DataFilePath = @"C:\Users\Thiago\source\repos\DotSights\DotSights.Tracker\DotSightsData.json";
		var result = DotSights.GetDataFromDataPath();

		var grouped = ActivityData.GroupByRule(result, new() {Name = "Test", RegexQuery = @"\bDotSights\b" });

		foreach (var item in grouped)
		{
			Console.WriteLine(item);
		}
	}
}

