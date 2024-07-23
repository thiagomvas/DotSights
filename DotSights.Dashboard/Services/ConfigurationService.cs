using DotSights.Core.Common.Types;

namespace DotSights.Dashboard.Services
{
    public class ConfigurationService
    {

        private static ConfigurationService _instance;
        private static readonly object _lock = new object();

        private ConfigurationService()
        {
            // Private constructor to prevent instantiation
        }

        public static ConfigurationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigurationService();
                        }
                    }
                }
                return _instance;
            }
        }

        public DotSightsSettings DashboardSettings = new();
        public bool HasLoaded = false;

        // Save settings file to json
        public void SaveSettings(DotSightsSettings settings)
        {
            DashboardSettings = settings;
            Core.DotSights.SaveSettings(settings);
        }

        public DotSightsSettings LoadSettings()
        {
            var settings = Core.DotSights.LoadSettings();

            DashboardSettings = settings;
            HasLoaded = true;
            return settings;

        }
    }
}
