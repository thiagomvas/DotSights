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
		private Timer? _timer = null;

		int ciclesSinceSave = 0;
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
					activity++;
					trackedData.Add(currentWindow, activity);
				}

				if(ciclesSinceSave >= 5)
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
