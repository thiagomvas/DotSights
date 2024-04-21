using DotSights.Core.Common.Types;

namespace DotSights.Tracker
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private string logFilePath = "DotSightsLog.txt";
		private string dataFilePath = "DotSightsData.json";
		private static DateTime startTime;
		private Dictionary<string, ActivityData> trackedData = new();

		int ciclesSinceSave = 0; // 1 cycle = 1 second
		int saveCicleDelay = 60 * 5; // Save every 5 minutes ( 60 seconds * 5 )
		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			startTime = DateTime.Now;

			if (!File.Exists(dataFilePath))
			{
				File.Create(dataFilePath).Close();
			}

			var readJson = File.ReadAllText(dataFilePath);


			if (Core.DotSights.DeserializeData(readJson, out var loadedData))
			{
				foreach (var activity in loadedData!)
				{
					trackedData.Add(activity.WindowTitle, activity);
				}
			}

			while (!stoppingToken.IsCancellationRequested)
			{
				string currentWindow = Core.DotSights.GetFocusedWindow();
				if (trackedData.ContainsKey(currentWindow))
				{
					trackedData[currentWindow]++;
				}
				else
				{
					var activity = new ActivityData(currentWindow);
					activity.ProcessName = Core.DotSights.GetFocusedProcessName();
					activity++;
					trackedData.Add(currentWindow, activity);
				}

				if(ciclesSinceSave >= saveCicleDelay)
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
			File.WriteAllText(dataFilePath, json);
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			SaveData(null);
			return base.StopAsync(cancellationToken);
		}
	}
}
