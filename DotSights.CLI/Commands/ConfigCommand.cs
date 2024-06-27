using DotSights.Core.Common.Types;
using System.CommandLine;
using System.Diagnostics;
using static DotSights.Core.DotSights;

namespace DotSights.CLI.Commands
{
	internal class ConfigCommand : BaseCommand
	{
		public ConfigCommand() : base("config", "Configuration-Related commands for DotSights")
		{
		}

		public override void Setup(RootCommand root)
		{
			AddCommand(new ResetCommand());
			AddCommand(new WipeDataCommand());
			AddCommand(new OpenDataCommand());

			this.SetHandler(OpenConfigFile);

			root.Add(this);
		}

		private void OpenConfigFile()
		{
			try
			{
				Process.Start(new ProcessStartInfo(SettingsFilePath) { UseShellExecute = true });
				Console.WriteLine("Opening config file...");
			}
			catch
			{
				Console.WriteLine("Failed to open config file.");
			}
		}

		private class ResetCommand : Command
		{
			public ResetCommand() : base("reset", "Resets the configuration file to default values")
			{
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				SaveSettings(new DotSightsSettings());
				Console.WriteLine("Saved default settings to configuration file.");
			}
		}

		private class WipeDataCommand : Command
		{
			public WipeDataCommand() : base("wipedata", "Wipes all data tracked by DotSights")
			{
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				// Request confirmation
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("WARNING: This action cannot be undone.");
				Console.ResetColor();
				Console.Write("Are you sure you want to wipe all data? ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("(y/n): ");
				Console.ResetColor();
				if (Console.ReadLine().ToLower() != "y")
				{
					Console.WriteLine("Aborted.");
					return;
				}

				var processes = Process.GetProcessesByName("DotSights.Tracker");
				foreach(var process in processes)
				{
					try
					{
						process.Kill();
						process.WaitForExit();
					}
					catch (Exception ex)
					{

						Console.WriteLine($"Failed to kill tracker process: {ex.Message}");
					}
				}

				SaveDataToDataPath(new());


				Console.WriteLine("Wiped all data.");
				var trackerExeLocation = Path.GetFullPath(LoadSettings().TrackerExeLocation);
				try
				{
					if (File.Exists(trackerExeLocation))
					{
						Console.WriteLine($"Restarting Tracker");
						var process = Process.Start(new ProcessStartInfo(trackerExeLocation) { UseShellExecute = true });
						if (process != null)
						{
							Console.WriteLine($"Restarted Tracker successfully");
						}
						else
						{
							Console.WriteLine("Process start returned null.");
						}
					}
					else
					{
						Console.WriteLine($"Executable not found at {trackerExeLocation}");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to start process: {ex.Message}");
				}
			}
		}

		private class OpenDataCommand : Command
		{
			public OpenDataCommand() : base("opendata", "Opens the data folder in the file explorer")
			{
				this.SetHandler(Execute);
			}

			private void Execute()
			{
				try
				{
					Process.Start(new ProcessStartInfo(DataFilePath) { UseShellExecute = true });
					Console.WriteLine("Opening data folder...");
				}
				catch
				{
					Console.WriteLine("Failed to open data folder.");
				}
			}
		}
	}
}
