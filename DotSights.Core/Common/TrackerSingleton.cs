using DotSights.Core.Common.Types;
using System.Text.Json;

namespace DotSights.Core.Common
{
	public class TrackerSingleton
	{
		private static string logFolderPath = @"C:\Users\Thiago"; // Define your log folder path here
		private static string logFilePath = Path.Combine(logFolderPath, "WindowFocusLog.txt");
		private static string dataFilePath = Path.Combine(logFolderPath, "WindowFocusData.txt");
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
				foreach (var activity in loadedData)
				{
					trackedData.Add(activity.WindowTitle, activity);
				}
			}

			if (!Directory.Exists(logFolderPath))
				Directory.CreateDirectory(logFolderPath);

			while ((DateTime.Now - startTime).TotalSeconds <= 10)
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
				LogWindow(currentWindow);
				Thread.Sleep(1000);
			}

			var json = DotSights.SerializeData(trackedData.Values.ToList());
			File.WriteAllText(dataFilePath, json);
		}


		private static void LogWindow(string windowTitle)
		{
			// Append the current window title to the log file
			using (StreamWriter sw = File.AppendText(logFilePath))
			{
				sw.WriteLine($"{DateTime.Now}: {windowTitle}");
			}
		}
	}
}
