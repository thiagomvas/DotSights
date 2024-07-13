using DotSights.Core.Common.Types;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

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
            settings.TrackerExeLocation = Process.GetCurrentProcess().MainModule.FileName;
            Core.DotSights.SaveSettings(settings);
            // Create %AppData%\DotSights folder if it doesn't exist and save data file to that folder
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var lastSaveTime = DateTime.Now;
                Core.DotSights.AssureSetup();


                while (!stoppingToken.IsCancellationRequested)
                {
                    long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    string currentWindow = Core.DotSights.GetFocusedWindow();
                    string searchKey = currentWindow;
                    var processName = Core.DotSights.GetFocusedProcessName().ToLower();
                    if (settings.OptimizeForStorageSpace && settings.GroupedProcessNames.Any(p => string.Equals(p, processName, StringComparison.OrdinalIgnoreCase)))
                    {
                        var match = trackedData.Values.Where(x => x.ProcessName.ToLower() == processName.ToLower()).FirstOrDefault();
                        if (match != null)
                        {
                            searchKey = match.ProcessName;
                        }
                    }

                    if (trackedData.ContainsKey(searchKey))
                    {
                        trackedData[searchKey]++;
                    }
                    else
                    {
                        RegisterNewActivity(searchKey);
                    }

                    if (DateTime.Now - lastSaveTime >= settings.TrackerSaveInterval)
                    {
                        settings = Core.DotSights.LoadSettings();

                        await SaveData();
                        lastSaveTime = DateTime.Now;

                        // Check if backup is needed
                        if (DateTime.Now - Core.DotSights.GetLastBackupDate() >= settings.DataBackupInterval)
                        {
                            Core.DotSights.CreateNewDataBackup();
                            Core.DotSights.AssureBackupFileCount(settings.MaxBackupFileCount);
                        }
                    }

                    long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    int delay = (int)Math.Max(1, 1000 - (end - start));
                    Thread.Sleep(delay);
                }
            }
            finally
            {
                await SaveData();
            }
        }
        private async Task SaveData()
        {
            Dictionary<string, ActivityData> data = new();
            var loadeddata = Core.DotSights.GetDataFromDataPath();

            if (loadeddata != null)
            {
                foreach (var activity in loadeddata)
                {
                    data.Add(activity.WindowTitle, activity);
                }
            }


            // Iterate over tracked data, and increment the total time for each activity
            foreach (var (key, activity) in trackedData)
            {
                if (settings.OptimizeForStorageSpace && settings.GroupedProcessNames.Any(p => string.Equals(p, activity.ProcessName, StringComparison.OrdinalIgnoreCase)))
                {
                    var match = data.Values.Where(x => x.ProcessName.ToLower() == activity.ProcessName.ToLower()).FirstOrDefault();
                    if (match != null)
                    {
                        data[match.WindowTitle] += activity;
                    }
                    else
                    {
                        data.Add(key, activity);
                    }
                }
                else
                {
                    if (data.ContainsKey(key))
                    {
                        data[key] += activity;
                    }
                    else
                    {
                        data.Add(key, activity);
                    }
                }

            }



            var json = Core.DotSights.SerializeData(data.Values.ToList());
            await File.WriteAllTextAsync(Core.DotSights.DataFilePath, json);
            trackedData.Clear();
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            SaveData();
            return base.StopAsync(cancellationToken);
        }

        private void RegisterNewActivity(string windowTitle)
        {
            var activity = new ActivityData(windowTitle);
            activity.ProcessName = Core.DotSights.GetFocusedProcessName();
            activity++;
            trackedData.Add(windowTitle, activity);
        }
    }
}
