using DotSights.Core;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using SharpTables.Graph;
using System.Globalization;

var data = DotSights.Core.DotSights.GetDataFromDataPath();
ActivityData total = data.Aggregate((acc, d) => acc + d);

Graph<KeyValuePair<DayOfWeek, int>> dowGraph = new Graph<KeyValuePair<DayOfWeek, int>>(total.UsageTimePerWeekDay.OrderBy(kv => kv.Key))
    .UseValueGetter(kv => kv.Value)
    .UseXTickFormatter(kv => DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(kv.Key))
    .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
    .UseHeader("Usage per day of week")
    .UseYAxisPadding(1);
dowGraph.Write();
Console.WriteLine();

bool is24Hour = DateTimeFormatInfo.CurrentInfo.ShortTimePattern.Contains("H");
var timePattern = is24Hour ? "HH" : "h tt";

// Ensure 24 entries
var hourlyUsage = total.UsageTimePerHour;
if (hourlyUsage.Count < 24)
{
    hourlyUsage = hourlyUsage.Concat(Enumerable.Range(0, 24).Except(hourlyUsage.Select(kv => kv.Key)).Select(h => new KeyValuePair<int, int>(h, 0))).OrderBy(kv => kv.Key).ToDictionary();
}



var todGraph = new Graph<KeyValuePair<int, int>>(hourlyUsage)
    .UseValueGetter(kv => kv.Value)
    .UseXTickFormatter(kv => DateTime.Today.AddHours(kv.Key).ToString(timePattern))
    .UseYTickFormatter(v => DotFormatting.FormatTimeShort((int)v))
    .UseMaxValue(total.UsageTimePerHour.MaxBy(kvp => kvp.Value).Value * 1.1)
    .UseMinValue(total.UsageTimePerHour.MinBy(kvp => kvp.Value).Value)
    .UseHeader("Usage per hour of day")
    .UseYAxisPadding(1);

foreach(var page in todGraph.ToPaginatedGraph(12))
{
    page.Write();
    Console.WriteLine();
}