using DotSights.Core.Common;
using DotSights.Core.Common.Types;
using SharpTables;
using SharpTables.Graph;
using System.Globalization;

var settings = new DotSightsSettings { GroupedProcessNames = ["process1"], OptimizeForStorageSpace = true };

var grouped1 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window1", FocusedTimeInSeconds = 10 };
var grouped2 = new ActivityData { ProcessName = "Process1", WindowTitle = "Window2", FocusedTimeInSeconds = 10 };
var ungrouped1 = new ActivityData { ProcessName = "Process2", WindowTitle = "Window3", FocusedTimeInSeconds = 10 };
var ungrouped2 = new ActivityData { ProcessName = "Process3", WindowTitle = "Window4", FocusedTimeInSeconds = 10 };

var db = new DotsightsDB(settings);
db.AddData(grouped1);
db.AddData(grouped2);
db.AddData(ungrouped1);
db.AddData(ungrouped2);

Table.FromDataSet(db.Activities, a =>
{
    return new Row(a.WindowTitle, a.ProcessName, a.FocusedTimeInSeconds.ToString(CultureInfo.InvariantCulture));
})
    .SetHeader(new("Window Title", "Process Name", "Focused Time (s)"))
    .UseFormatting(TableFormatting.ASCII)
    .Write();
