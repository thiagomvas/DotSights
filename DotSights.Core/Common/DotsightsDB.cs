using DotSights.Core.Common.Types;

namespace DotSights.Core.Common
{
    public class DotsightsDB
    {
        private DotSightsSettings _settings;
        private List<ActivityData> activities;
        private List<DailyData> dailyDatas;

        public List<ActivityData> Activities => activities.ToList();
        public List<DailyData> DailyDatas => dailyDatas.ToList();

        public DotsightsDB()
        {
            _settings = DotSights.LoadSettings();
            activities = new List<ActivityData>();
            dailyDatas = new List<DailyData>();
        }

        public void LoadDataFromFile()
        {
            activities = DotSights.GetDataFromDataPath();
            dailyDatas = DotSights.GetDailyDataFromDataPath();
        }

        public void AddData(ActivityData data)
        {

            DailyData today = dailyDatas.FirstOrDefault(d => d.Date.Date == DateTime.Now.Date);
            if (today == null)
            {
                today = new DailyData { Date = DateTime.Now.Date };
                dailyDatas.Add(today);
            }
            
            // Ensure only track 7 days of data
            if (dailyDatas.OrderBy(d => d.Date).Count() > 7)
            {
                dailyDatas.RemoveAt(0);
            }


            ActivityData match = null;
            if(_settings.OptimizeForStorageSpace)
            {
                match = activities.Where(x => x.ProcessName.ToLower() == data.ProcessName.ToLower()).FirstOrDefault();
            }
            else
            {
                match = activities.FirstOrDefault(x => x.WindowTitle == data.WindowTitle);
            }


            // Get delta time for each hour
            Dictionary<int, int> increments = new();
            foreach (var (hour, time) in data.UsageTimePerHour)
            {
                // Compare with existing data
                if (match != null)
                {
                    if (match.UsageTimePerHour.ContainsKey(hour))
                    {
                        increments[hour] = data.UsageTimePerHour[hour];
                    }
                    else
                    {
                        increments[hour] = time;
                    }
                }
                else
                {
                    increments[hour] = time;
                }
            }

            // Update the data
            foreach(var (hour, time) in increments)
            {
                if (today.UsageTimePerHour.ContainsKey(hour))
                {
                    today.UsageTimePerHour[hour] += time;
                }
                else
                {
                    today.UsageTimePerHour[hour] = time;
                }
            }


            if (match != null)
            {
                activities[activities.IndexOf(match)] += data;
            }
            else
            {
                activities.Add(data);
            }

        }

        public void SaveChanges()
        {
            var json = DotSights.SerializeData(activities);
            File.WriteAllText(DotSights.DataFilePath, json);

            var json2 = DotSights.SerializeData(dailyDatas);
            File.WriteAllText(DotSights.DailyDataFilePath, json2);
        }
    }
}
