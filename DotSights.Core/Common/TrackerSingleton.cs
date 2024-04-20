using DotSights.Core.Common.Types;
using System.Diagnostics;
using System.Text.Json;

namespace DotSights.Core.Common
{
	public class TrackerSingleton
	{
		private static string logFilePath = "DotSightsLog.txt";
		private static string dataFilePath = "DotSightsData.json";
		private static DateTime startTime;
		private static TrackerSingleton instance;
		private TrackerSingleton() { }

		public static TrackerSingleton Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new TrackerSingleton();
				}
				return instance;
			}
		}

		public void Track()
		{
			startTime = DateTime.Now;

			if(!File.Exists(dataFilePath))
			{
				File.Create(dataFilePath).Close();
			}

			var readJson = File.ReadAllText(dataFilePath);


			Dictionary<string, ActivityData> trackedData = new();

			if (DotSights.DeserializeData(readJson, out var loadedData))
			{
				foreach (var activity in loadedData!)
				{
					trackedData.Add(activity.WindowTitle, activity);
				}
			}


			while (true)
			{
				string currentWindow = DotSights.GetFocusedWindow();
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
				Thread.Sleep(1000);
			}

			var json = DotSights.SerializeData(trackedData.Values.ToList());
			File.WriteAllText(dataFilePath, json);
		}
	}
}
