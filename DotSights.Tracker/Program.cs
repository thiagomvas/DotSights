using DotSights.Core.Common.Types;
using System.Runtime.InteropServices;
using DotSights.Core;
namespace DotSights.Tracker
{
	class Program
	{
		private static string logFolderPath = @"C:\Users\Thiago"; // Define your log folder path here
		private static string logFilePath = Path.Combine(logFolderPath, "WindowFocusLog.txt");
		private static DateTime startTime;
		private static bool isRunning;

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowText(IntPtr hWnd, string lpString, int nMaxCount);

		static void Main(string[] args)
		{
			isRunning = true;
			startTime = DateTime.Now;
			var data = Core.DotSights.DeserializeActivityDataList(File.ReadAllText(logFilePath));

			Dictionary<string, ActivityData> windowFocusCount = new();
			if (data != null)
			{
				foreach (var activityData in data)
				{
					windowFocusCount.Add(activityData.WindowTitle, activityData);
				}
			}

			Directory.CreateDirectory(logFolderPath);

			while ((DateTime.Now - startTime).TotalSeconds <= 10)
			{
				string currentWindow = Core.DotSights.GetFocusedWindow();
				if (windowFocusCount.ContainsKey(currentWindow))
				{
					windowFocusCount[currentWindow]++;
				}
				else
				{
					windowFocusCount.Add(currentWindow, new(currentWindow));
				}
				Console.WriteLine($"{currentWindow}");
				Thread.Sleep(1000);
			}

			isRunning = false;
			foreach (var kvp in windowFocusCount)
			{
				Console.WriteLine($"{kvp.Value}");
			}

			var json = Core.DotSights.SerializeActivityData(windowFocusCount.Values.ToList());
			File.WriteAllText(logFilePath, json);
			Console.ReadLine();
		}


		private static void LogWindow(string windowTitle)
		{
			// Append the current window title to the log file
			using (StreamWriter sw = File.AppendText(logFilePath))
			{
				sw.WriteLine($"{DateTime.Now}: {windowTitle}");
			}
		}

		private static void WriteLogFile()
		{
			// Write the collected data to a file
			Console.WriteLine("Writing log file...");
			// You can do additional processing here if needed
			Console.WriteLine("Log file written to: " + logFilePath);
		}
	}


}
