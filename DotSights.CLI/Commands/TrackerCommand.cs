using System.CommandLine;
using System.Diagnostics;
using static DotSights.Core.DotSights;

namespace DotSights.CLI.Commands
{
    internal class TrackerCommand : BaseCommand
    {
        public TrackerCommand() : base("tracker", "Tracker controls")
        {
        }

        public override void Setup(RootCommand root)
        {
            AddCommand(new StartCommand());
            AddCommand(new StopCommand());

            root.Add(this);
        }

        private class StartCommand : Command
        {
            public StartCommand() : base("start", "Starts the tracker if it isn't already running")
            {
                this.SetHandler(Execute);
            }

            private void Execute()
            {
                // Check if DotSights.Tracker is running

                var processes = Process.GetProcessesByName("DotSights.Tracker");
                if (processes.Length > 0)
                {
                    Console.WriteLine("The tracker is already running.");
                    return;
                }


                var trackerExeLocation = Path.GetFullPath(LoadSettings().TrackerExeLocation);
                var process = Process.Start(new ProcessStartInfo(trackerExeLocation) { UseShellExecute = true });

            }
        }

        private class StopCommand : Command
        {
            public StopCommand() : base("stop", "Stops the tracker if it is running")
            {
                this.SetHandler(Execute);
            }

            private void Execute()
            {
                // Ask for confirmation
                Console.WriteLine("Shutting down the tracker means the data tracked since the previous save will be lost.");
                Console.WriteLine("Are you sure you want to stop the tracker? (y/n)");

                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Y)
                {
                    Console.WriteLine("Aborted.");
                    return;
                }

                var processes = Process.GetProcessesByName("DotSights.Tracker");
                foreach (var process in processes)
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
            }
        }
    }
}
