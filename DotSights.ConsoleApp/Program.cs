using DotSights.Core;
using DotSights.Core.Common;
using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using SharpTables;
using SharpTables.Graph;
using System.Globalization;

var db = new DotsightsDB();

var data1 = new ActivityData
{
    WindowTitle = "Visual Studio Code",
    ProcessName = "code",
    FocusedTimeInSeconds = (int) TimeSpan.FromMinutes(1).TotalSeconds,
};

var data2 = new ActivityData
{
    WindowTitle = "Window 2",
    ProcessName = "data2",
    FocusedTimeInSeconds = (int) TimeSpan.FromMinutes(1).TotalSeconds,
};
var data3 = new ActivityData
{
    WindowTitle = "Visual Studio Code",
    ProcessName = "code",
    FocusedTimeInSeconds = (int)TimeSpan.FromMinutes(3).TotalSeconds,
};

db.AddData(data1);
db.AddData(data2);

Table.FromDataSet(db.Activities, a =>
{
    return new Row(a.WindowTitle, a.ProcessName, a.FocusedTimeInSeconds.ToString(CultureInfo.InvariantCulture));
})
    .SetHeader(new("Window Title", "Process Name", "Focused Time (s)"))
    .UseFormatting(TableFormatting.ASCII)
    .Write();


db.AddData(data3);

Table.FromDataSet(db.Activities, a =>
{
    return new Row(a.WindowTitle, a.ProcessName, a.FocusedTimeInSeconds.ToString(CultureInfo.InvariantCulture));
})
    .SetHeader(new("Window Title", "Process Name", "Focused Time (s)"))
    .UseFormatting(TableFormatting.ASCII)
    .Write();
