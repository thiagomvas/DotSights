using DotSights.Core.Common;
using DotSights.Core.Common.Types;
using SharpTables;
using SharpTables.Graph;
using System.Globalization;

var db = new DotsightsDB();
db.LoadDataFromFile();

var data1 = new ActivityData
{
    WindowTitle = "Visual Studio Code",
    ProcessName = "code",
    FocusedTimeInSeconds = (int)TimeSpan.FromMinutes(1).TotalSeconds,
    UsageTimePerHour = { { 10, 60 } },
};

var data2 = new ActivityData
{
    WindowTitle = "Window 2",
    ProcessName = "data2",
    FocusedTimeInSeconds = (int)TimeSpan.FromMinutes(1).TotalSeconds,
};
var data3 = new ActivityData
{
    WindowTitle = "Visual Studio Code",
    ProcessName = "code",
    FocusedTimeInSeconds = (int)TimeSpan.FromMinutes(3).TotalSeconds,
    UsageTimePerHour = { { 11, 180 } },
};

db.AddData(data1);
db.AddData(data2);
db.AddData(data3);

Table.FromDataSet(db.Activities, a =>
{
    return new Row(a.WindowTitle, a.ProcessName, a.FocusedTimeInSeconds.ToString(CultureInfo.InvariantCulture));
})
    .SetHeader(new("Window Title", "Process Name", "Focused Time (s)"))
    .UseFormatting(TableFormatting.ASCII)
    .Write();

new Graph<KeyValuePair<int, int>>(db.DailyDatas[0].UsageTimePerHour)
    .UseValueGetter(kvp => kvp.Value)
    .UseXTickFormatter(kvp => kvp.Key.ToString("0"))
    .UseYTickFormatter(v => v.ToString("0"))
    .Write();

db.SaveChanges();