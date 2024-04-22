using DotSights.Dashboard.Models;
using Newtonsoft.Json;
using System.IO;

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

		public DashboardSettings DashboardSettings = new();
		public bool HasLoaded = false;

		// Save settings file to json
		public void SaveSettings(DashboardSettings settings)
		{
			DashboardSettings = settings;
			var json = JsonConvert.SerializeObject(settings);
			File.WriteAllText("settings.json", json);
		}

		public DashboardSettings LoadSettings()
		{
			if (!File.Exists("settings.json"))
			{
				DashboardSettings = new DashboardSettings();
				return new DashboardSettings();
			}

			var json = File.ReadAllText("settings.json");

			var settings = JsonConvert.DeserializeObject<DashboardSettings>(json);
			if(settings == null)
			{
				DashboardSettings = new DashboardSettings();
				return new DashboardSettings();
			}
			
			DashboardSettings = settings;
			HasLoaded = true;
			return settings;

		}
	}
}
