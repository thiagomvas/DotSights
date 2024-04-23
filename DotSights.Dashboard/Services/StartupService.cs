using System;
using System.IO;

namespace DotSights.Dashboard.Services
{
	internal class StartupService
	{
		public static void SetStartup()
		{
			string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			string shortcutPath = System.IO.Path.Combine(startupFolder, "DotSights.lnk");

			if (!System.IO.File.Exists(shortcutPath))
			{
				string trackerExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DotSights.Tracker.exe");

				if(!File.Exists(trackerExePath))
				{
					return;
				}

				var shell = new IWshRuntimeLibrary.WshShell();
				var shortcut = shell.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
				shortcut.TargetPath = trackerExePath;
				shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				shortcut.Description = "DotSights";
				shortcut.Save();
			}

		}

		public static void RemoveStartup()
		{
			string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			string shortcutPath = System.IO.Path.Combine(startupFolder, "DotSights.lnk");

			if (System.IO.File.Exists(shortcutPath))
			{
				System.IO.File.Delete(shortcutPath);
			}
		}

		public static bool IsStartup()
		{
			string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			string trackerExePath = Path.Combine(startupPath, "DotSights.lnk");
			return File.Exists(trackerExePath);
		}
	}
}
