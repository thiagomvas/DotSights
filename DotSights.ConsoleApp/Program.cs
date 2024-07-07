
namespace DotSights.ConsoleApp;

using DotSights.Core;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using SharpTables;

class Program
{
	public static void Main(string[] args)
	{
		var result = DotSights.GetDataFromDataPath();
		DotSightsSettings settings = DotSights.LoadSettings();
		//
		result = DotSights.SquashDataUsingRule(result, new GroupingRule() { Name = "Basalto", RegexQuery = ".*Basalt.*" }, true);
		
		
		result = DotSights.FilterDataFromSettings(result, settings);
		result = result.OrderBy(d => d.WindowTitle).ToList();
		Table t = Table.FromDataSet(result, d => new Row(d.WindowTitle.Length > 50 ? d.WindowTitle.Substring(0, 50) : d.WindowTitle, DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds)));
		t.SetHeader(new("Name", "Time"));
		t.Print();

	}
}

