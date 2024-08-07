﻿using DotSights.Core.Common.Types;
using DotSights.Core.Common.Utils;
using SharpTables;
using System.Collections;
using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using static DotSights.Core.DotSights;

namespace DotSights.CLI.Commands
{
    internal class ConfigCommand : BaseCommand
    {
        public ConfigCommand() : base("config", "Configuration-Related commands and options for DotSights. Some settings can only be changed by editing the config file directly. Configuration changes only apply after data save or restarting tracker.")
        {
        }

        public override void Setup(RootCommand root)
        {
            AddCommand(new ResetCommand());
            AddCommand(new WipeDataCommand());
            AddCommand(new OpenDataCommand());
            AddCommand(new PreviewCommand());
            AddCommand(new RegexCommand());
            AddCommand(new OpenCommand());
            AddCommand(new SquashCommand());
            AddCommand(new RestoreBackupCommand());
            AddCommand(new CreateBackupCommand());

            var groupItemsWithSameProcessName = new Option<bool?>(new[] { "--groupprocesses", "-gp" }, "Group items with the same process name");
            var groupItemsUsingGroupingRules = new Option<bool?>(new[] { "--userules", "-u" }, "Group items using grouping rules");
            var showOnlyRegexMatchedItems = new Option<bool?>(new[] { "--showregex", "-r" }, "Show only regex matched items");
            var regexMatchProcessName = new Option<bool?>(new[] { "--regexprocess", "-rp" }, "Regex Grouping Rules should match process names.");
            var trackerSaveInterval = new Option<int?>(new[] { "--saveinterval", "-si" }, "Interval in minutes to save tracker data");
            var optimizeForStorageSpace = new Option<bool?>(new[] { "--optimize", "-o" }, "Optimize for storage space");
            var maxBackupFileCount = new Option<int?>(new[] { "--maxbackup", "-mb" }, "Maximum number of backup files to keep");
            var backupInterval = new Option<int?>(new[] { "--backupinterval", "-bi" }, "Interval in days to backup data");

            this.AddOption(groupItemsWithSameProcessName);
            this.AddOption(groupItemsUsingGroupingRules);
            this.AddOption(showOnlyRegexMatchedItems);
            this.AddOption(regexMatchProcessName);
            this.AddOption(trackerSaveInterval);
            this.AddOption(optimizeForStorageSpace);
            this.AddOption(maxBackupFileCount);
            this.AddOption(backupInterval);

            this.SetHandler(Execute,
                   groupItemsWithSameProcessName,
                   groupItemsUsingGroupingRules,
                   showOnlyRegexMatchedItems,
                   regexMatchProcessName,
                   trackerSaveInterval,
                   optimizeForStorageSpace,
                   maxBackupFileCount,
                   backupInterval);

            root.Add(this);
        }

        private void Execute(bool? groupItemsWithSameProcessName,
                       bool? groupItemsUsingGroupingRules,
                       bool? showOnlyRegexMatchedItems,
                       bool? regexMatchProcessName,
                       int? trackerSaveInterval,
                       bool? optimizeForStorageSpace,
                       int? maxBackupFileCount,
                       int? backupInterval)
        {
            if (groupItemsWithSameProcessName == null
                && groupItemsUsingGroupingRules == null
                && showOnlyRegexMatchedItems == null
                && regexMatchProcessName == null
                && trackerSaveInterval == null
                && optimizeForStorageSpace == null
                && maxBackupFileCount == null
                && backupInterval == null)
            {
                Console.WriteLine("No changes specified.");
                return;
            }
            var settings = LoadSettings();
            if (groupItemsWithSameProcessName != null)
                settings.GroupItemsWithSameProcessName = groupItemsWithSameProcessName.Value;

            if (groupItemsUsingGroupingRules != null)
                settings.GroupItemsUsingGroupingRules = groupItemsUsingGroupingRules.Value;

            if (showOnlyRegexMatchedItems != null)
                settings.ShowOnlyRegexMatchedItems = showOnlyRegexMatchedItems.Value;

            if (regexMatchProcessName != null)
                settings.RegexMatchProcessName = regexMatchProcessName.Value;

            if (trackerSaveInterval != null)
                settings.TrackerSaveInterval = TimeSpan.FromMinutes(trackerSaveInterval.Value);

            if (optimizeForStorageSpace != null)
                settings.OptimizeForStorageSpace = optimizeForStorageSpace.Value;

            if (maxBackupFileCount != null)
                settings.MaxBackupFileCount = maxBackupFileCount.Value;

            if (backupInterval != null)
                settings.DataBackupInterval = TimeSpan.FromDays(backupInterval.Value);


            SaveSettings(settings);
            Console.WriteLine("Saved settings.");
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
                if (Console.ReadLine()?.ToLower() != "y")
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

        private class PreviewCommand : Command
        {
            public PreviewCommand() : base("preview", "Preview the configuration file")
            {
                this.SetHandler(Execute);
            }

            private void Execute()
            {
                var settings = LoadSettings();

                PropertyInfo[] props = settings.GetType().GetProperties().Where(p => !(typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string))).ToArray();
                Table t = Table.FromDataSet(props, p =>
                {
                    return new Row(p.Name, p.GetValue(settings)?.ToString() ?? string.Empty);
                });

                t.UseNullOrEmptyReplacement("N/A");
                t.SetHeader("Name", "Value");
                t.Header.Cells.ForEach(c => c.Alignment = Alignment.Center);


                t.AddRow("Grouped Processes", string.Join(',', settings.GroupedProcessNames));

                t.UsePreset(c =>
                {
                    if (bool.TryParse(c.Text, out bool b))
                    {
                        c.Color = b ? ConsoleColor.Green : ConsoleColor.Red;
                    }
                    else if (Path.Exists(c.Text))
                    {
                        c.Color = ConsoleColor.Cyan;
                    }
                    else if (c.Position.X == 0)
                        c.Color = ConsoleColor.Yellow;

                });

                Console.WriteLine("General Settings");
                t.Write();
                Console.WriteLine();

                Table groupingTable = Table.FromDataSet(settings.GroupingRules, r =>
                {
                    return new Row(r.Name, r.RegexQuery, r.ShowOnDashboard);
                });

                groupingTable.UseNullOrEmptyReplacement("N/A");
                groupingTable.SetHeader("Name", "Regex Query", "Display");
                groupingTable.UsePreset(c =>
                {
                    if (c.Position.X == 2)
                    {
                        c.Color = c.Text[0] == 'T' ? ConsoleColor.Green : ConsoleColor.Red;
                        c.Alignment = Alignment.Center;
                    }
                });

                Console.WriteLine("Grouping Rules");
                groupingTable.Write();
                Console.WriteLine();
            }
        }

        private class RegexCommand : Command
        {
            public RegexCommand() : base("regex", "Add or remove regex grouping rules")
            {
                this.AddCommand(new RegexAddCommand());
                this.AddCommand(new RegexRemoveCommand());
            }
            private class RegexAddCommand : Command
            {
                public RegexAddCommand() : base("add", "Add a new regex grouping rule")
                {
                    var optionName = new Option<string>(new[] { "--name", "-n" }, "Name of the rule");
                    var optionRegex = new Option<string>(new[] { "--regex", "-r" }, "Regex query to match");
                    var optionShowOnDashboard = new Option<bool>(new[] { "--show", "-s" }, "Whether to display the results or hide them.");

                    this.AddOption(optionName);
                    this.AddOption(optionRegex);
                    this.AddOption(optionShowOnDashboard);

                    this.SetHandler(Execute, optionName, optionRegex, optionShowOnDashboard);
                }

                private void Execute(string name, string regex, bool showOnDashboard)
                {
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(regex))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Name and regex query cannot be empty.");
                        Console.ResetColor();
                        return;
                    }
                    var settings = LoadSettings();
                    settings.GroupingRules.Add(new() { Name = name, RegexQuery = regex, ShowOnDashboard = showOnDashboard });
                    SaveSettings(settings);
                    Console.WriteLine("Added new regex rule.");
                }
            }

            private class RegexRemoveCommand : Command
            {
                public RegexRemoveCommand() : base("remove", "Remove a regex grouping rule")
                {
                    var optionName = new Option<string>(new[] { "--name", "-n" }, "Name of the rule");

                    this.AddOption(optionName);

                    this.SetHandler(Execute, optionName);
                }

                private void Execute(string name)
                {
                    name = name.Trim().ToLowerInvariant();
                    var settings = LoadSettings();
                    var rule = settings.GroupingRules.FirstOrDefault(r => r.Name.Trim().ToLowerInvariant() == name);
                    if (rule != null)
                    {
                        settings.GroupingRules.Remove(rule);
                        SaveSettings(settings);
                        Console.WriteLine("Removed regex rule.");
                    }
                    else
                    {
                        Console.WriteLine("Rule not found.");
                    }
                }
            }
        }

        private class OpenCommand : Command
        {
            public OpenCommand() : base("open", "Opens the configuration file in the default text editor")
            {
                this.SetHandler(Execute);
            }

            private void Execute()
            {
                try
                {
                    Process.Start(new ProcessStartInfo(SettingsFilePath) { UseShellExecute = true });
                    Console.WriteLine("Opening configuration file...");
                }
                catch
                {
                    Console.WriteLine("Failed to open configuration file.");
                }
            }
        }

        private class SquashCommand : Command
        {
            public SquashCommand() : base("squash", "Squash data using grouping rules")
            {
                var optionName = new Option<string?>(new[] { "--name", "-n" }, "Name of the rule");
                var optionRegex = new Option<string?>(new[] { "--regex", "-r" }, "Regex query to match");
                var optionMatchProcessNames = new Option<bool>(new[] { "--matchprocess", "-mp" }, "Match process names");

                this.AddOption(optionName);
                this.AddOption(optionRegex);
                this.AddOption(optionMatchProcessNames);

                this.SetHandler(Execute, optionName, optionRegex, optionMatchProcessNames);

            }

            private void Execute(string? name, string? regex, bool matchProcessNames)
            {
                if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(regex))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Name and regex query cannot be empty.");
                    Console.ResetColor();
                    return;
                }

                // See if tracker is running
                var processes = Process.GetProcessesByName("DotSights.Tracker");
                if (processes.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Tracker is running. Please stop the tracker before squashing data.");
                    Console.ResetColor();
                    return;
                }

                var data = GetDataFromDataPath();
                var rule = new GroupingRule() { Name = name, RegexQuery = regex, ShowOnDashboard = true };

                // Display entries to be squashed
                List<ActivityData> toSquash;

                if (matchProcessNames)
                {
                    toSquash = data.Where(d => Regex.IsMatch(d.ProcessName, regex) || Regex.IsMatch(d.WindowTitle, regex)).ToList();
                }
                else
                {
                    toSquash = data.Where(d => Regex.IsMatch(d.WindowTitle, regex)).ToList();
                }

                if (toSquash.Count == 0)
                {
                    Console.WriteLine("No entries to squash.");
                    return;
                }

                Console.WriteLine("Entries to squash:");
                Table t = Table.FromDataSet(toSquash, d =>
                {
                    var name = d.WindowTitle.Length > 50 ? d.WindowTitle.Substring(0, 50) : d.WindowTitle;
                    var totaltime = DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds);
                    var timetoday = DotFormatting.FormatTimeShort(d.TotalTimeToday);
                    var timeweek = DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek());
                    return new Row(name, totaltime, timetoday, timeweek);
                });
                t.SetHeader("Name", "Total Time", "Today", "Week");
                t.Write();

                var squash = SquashDataUsingRule(data, rule, matchProcessNames);
                var result = squash.FirstOrDefault(d => d.WindowTitle == name);
                if (result != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Squashed entry:");
                    Table t2 = Table.FromDataSet(new[] { result }, d =>
                    {
                        var name = d.WindowTitle.Length > 50 ? d.WindowTitle.Substring(0, 50) : d.WindowTitle;
                        var totaltime = DotFormatting.FormatTimeShort((int)d.FocusedTimeInSeconds);
                        var timetoday = DotFormatting.FormatTimeShort(d.TotalTimeToday);
                        var timeweek = DotFormatting.FormatTimeShort(d.GetUsageTimeForWeek());
                        return new Row(name, totaltime, timetoday, timeweek);
                    });
                    t2.SetHeader("Name", "Total Time", "Today", "Week");
                    t2.Write();
                }
                else
                {
                    Console.WriteLine("Failed to squash entries.");
                }

                Console.WriteLine("This action is irreversible. Do you want to save the changes?");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("(y/n): ");
                Console.ResetColor();
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    SaveDataToDataPath(squash.ToList());
                    Console.WriteLine("Saved changes.");
                }
                else
                {
                    Console.WriteLine("Changes not saved.");
                }
            }
        }

        private class RestoreBackupCommand : Command
        {
            public RestoreBackupCommand() : base("restore", "Restore a backup file.")
            {
                this.SetHandler(Execute);
            }

            private void Execute()
            {
                var current = GetDataFromDataPath();
                var backupFiles = Directory.GetFiles(AppDataPath, "*.backup");
                if (backupFiles == null || backupFiles.Length == 0)
                {
                    Console.WriteLine("No backup files found.");
                    return;
                }

                var backupTotalData = new List<ActivityData>();
                foreach (var file in backupFiles)
                {
                    var data = GetDataFromBackup(file);
                    ActivityData total = data.Aggregate((a, b) => a + b);
                    backupTotalData.Add(total);
                }

                var table = new Table()
                    .DisplayRowIndexes()
                    .SetHeader(new Row("File Name", "Total time tracked for backup"));

                for (int i = 0; i < backupTotalData.Count; i++)
                {
                    table.AddRow(Path.GetFileName(backupFiles[i]), DotFormatting.FormatTimeShort((int)backupTotalData[i].FocusedTimeInSeconds));
                }
                table.AddRow(new Row("Current", DotFormatting.FormatTimeShort((int)current.Aggregate((a, b) => a + b).FocusedTimeInSeconds)));

                table.Write();

                Console.WriteLine("Enter the number of the backup file to restore:");
                Console.Write(">");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int index) && index > 0 && index <= backupTotalData.Count)
                {
                    var backupFile = backupFiles[index - 1];
                    var data = GetDataFromBackup(backupFile);
                    SaveDataToDataPath(data);
                    Console.WriteLine("Restored backup file.");
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
        }

        private class CreateBackupCommand : Command
        {
            public CreateBackupCommand() : base("backup", "Create a backup of the current data.")
            {
                this.SetHandler(Execute);
            }

            private void Execute()
            {
                var data = GetDataFromDataPath();
                var backupFile = Path.Combine(AppDataPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.backup");
                CreateNewDataBackup();
                Console.WriteLine($"Created backup file: {backupFile}");
            }
        }
    }
}
