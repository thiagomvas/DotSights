
namespace DotSights.ConsoleApp;

using DotSights.Core;
using DotSights.Core.Common;

class Program
{
	public static void Main(string[] args)
	{
		DotSights.DataFilePath = @"C:\Users\Thiago\source\repos\DotSights\DotSights.Tracker\DotSightsData.json";
		var result = DotSights.GetDataFromDataPath();

		DotSights.CreateDataCharts(result);
	}
}

