using DotSights.Core;
using DotSights.Core.Common.Types;
using DotSights.Dashboard.Models;

namespace DotSights.Tracker
{
	public class Worker : BackgroundService
	{
		private DotSightsSettings settings = new();
		private readonly ILogger<Worker> _logger;
		private static DateTime startTime;
		private Dictionary<string, ActivityData> trackedData = new();

		int ciclesSinceSave = 0; // 1 cycle = 1 second
		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
			settings = Core.DotSights.LoadSettings();
			// Create %AppData%\DotSights folder if it doesn't exist and save data file to that folder
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			startTime = DateTime.Now;
			Core.DotSights.AssureSetup();

			var data = Core.DotSights.GetDataFromDataPath();

			if(data == null)
			{
				data = new List<ActivityData>();
			}

			foreach (var activity in data!)
			{
				trackedData.Add(activity.WindowTitle, activity);
			}
			

			while (!stoppingToken.IsCancellationRequested)
			{
				string currentWindow = Core.DotSights.GetFocusedWindow();
				string searchKey = currentWindow;
				if(settings.OptimizeForStorageSpace)
				{
					string processName = Core.DotSights.GetFocusedProcessName();
					if(settings.GroupedProcessNames.Contains(processName))
					{
						searchKey = processName;
					}
				}
				if (trackedData.ContainsKey(searchKey))
				{
					trackedData[searchKey]++;
				}
				else
				{
					var activity = new ActivityData(currentWindow);
					activity.ProcessName = Core.DotSights.GetFocusedProcessName();
					activity++;
					trackedData.Add(searchKey, activity);
				}

				if (ciclesSinceSave >= settings.TrackerSaveInterval.TotalSeconds)
				{
					SaveData(null);
					ciclesSinceSave = 0;
				}

				ciclesSinceSave++;
				await Task.Delay(1000, stoppingToken);
			}
		}
		private void SaveData(object? state)
		{
			var json = Core.DotSights.SerializeData(trackedData.Values.ToList());
			File.WriteAllText(Core.DotSights.DataFilePath, json);
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			SaveData(null);
			return base.StopAsync(cancellationToken);
		}
	}
}
