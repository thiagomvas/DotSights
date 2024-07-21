using System.Runtime;

namespace DotSights.Core.Common.Types
{
    public class DailyData
    {
        public DateTime Date { get; set; }
        public Dictionary<int, int> UsageTimePerHour { get; set; } = new();

    }
}
