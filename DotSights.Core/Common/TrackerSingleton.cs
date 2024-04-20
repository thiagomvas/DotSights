using DotSights.Core.Common.Types;

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
			var loaded = File.ReadAllText(dataFilePath);


			Dictionary<string, ActivityData> windowFocusCount = new();

			Directory.CreateDirectory(logFolderPath);

			while ((DateTime.Now - startTime).TotalSeconds <= 10)
			{
				string currentWindow = DotSights.GetFocusedWindow();
				if (windowFocusCount.ContainsKey(currentWindow))
				{
					windowFocusCount[currentWindow]++;
				}
				else
				{
					windowFocusCount.Add(currentWindow, new(currentWindow, 1));

				}
				LogWindow(currentWindow);
				Thread.Sleep(1000);
			}

			var json = DotSights.SerializeActivityData(windowFocusCount.Values.ToList());
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
