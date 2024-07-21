using DotSights.Core.Common.Types;

namespace DotSights.Core.Common
{
    public class DotsightsDB
    {
        private DotSightsSettings _settings;
        private List<ActivityData> activities;

        public List<ActivityData> Activities => activities.ToList();

        public DotsightsDB()
        {
            _settings = DotSights.LoadSettings();
            activities = new List<ActivityData>();
        }

        public void LoadDataFromFile()
        {
            activities = DotSights.GetDataFromDataPath();
        }

        public void AddData(ActivityData data)
        {
            if(_settings.OptimizeForStorageSpace && _settings.GroupedProcessNames.Any(p => string.Equals(p, data.ProcessName, StringComparison.OrdinalIgnoreCase)))
            {
                var match = activities.Where(x => x.ProcessName.ToLower() == data.ProcessName.ToLower()).FirstOrDefault();
                if(match != null)
                {
                    activities[activities.IndexOf(match)] += data;
                }
                else
                {
                    activities.Add(data);
                }
            }
            else
            {
                if(activities.Contains(data))
                {
                    activities[activities.IndexOf(data)] += data;
                }
                else
                {
                    activities.Add(data);
                }
            }
        }

        public void SaveChanges()
        {
            var json = DotSights.SerializeData(activities);
            File.WriteAllText(DotSights.DataFilePath, json);
        }
    }
}
