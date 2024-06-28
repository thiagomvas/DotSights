using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotSights.Core.Common.Types;
using DotSights.Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DotSights.Dashboard.ViewModels
{
	public partial class SettingsPageViewModel : ViewModelBase
	{
		[ObservableProperty]
		private bool _groupItemsWithSameProcessName;

		[ObservableProperty]
		private bool _groupItemsUsingGroupingRules;

		[ObservableProperty]
		private TimeSpan _trackerSaveDelay;

		[ObservableProperty]
		private bool _runOnStartup;

		[ObservableProperty]
		private bool _optimizeForStorageSpace;

		[ObservableProperty]
		private bool _regexMatchesOnly;

		[ObservableProperty]
		private bool _regexMatchProcessName;


		public ObservableCollection<GroupingRule> GroupingRules { get => _groupingRules; set => this.SetProperty(ref _groupingRules, value); }
		private ObservableCollection<GroupingRule> _groupingRules = new();

		private ObservableCollection<ProcessName> _groupProcessNames = new();
		public ObservableCollection<ProcessName> GroupProcessNames { get => _groupProcessNames; set => this.SetProperty(ref _groupProcessNames, value); }

		public List<string> testlist = new();
		public SettingsPageViewModel()
		{
			var service = ConfigurationService.Instance;
			var settings = service.LoadSettings();
			GroupItemsWithSameProcessName = settings.GroupItemsWithSameProcessName;
			GroupItemsUsingGroupingRules = settings.GroupItemsUsingGroupingRules;
			GroupingRules = new(settings.GroupingRules);
			TrackerSaveDelay = settings.TrackerSaveInterval;
			OptimizeForStorageSpace = settings.OptimizeForStorageSpace;
			RegexMatchesOnly = settings.ShowOnlyRegexMatchedItems;
			RegexMatchProcessName = settings.RegexMatchProcessName;
			GroupProcessNames = new(settings.GroupedProcessNames.Select(x => new ProcessName(x)).ToList());

		}

		[RelayCommand]
		public void SaveSettings()
		{
			var service = ConfigurationService.Instance;
			var settings = new DotSightsSettings
			{
				GroupItemsWithSameProcessName = GroupItemsWithSameProcessName,
				GroupItemsUsingGroupingRules = GroupItemsUsingGroupingRules,
				GroupingRules = GroupingRules.ToList(),
				TrackerSaveInterval = TrackerSaveDelay,
				OptimizeForStorageSpace = OptimizeForStorageSpace,
				ShowOnlyRegexMatchedItems = RegexMatchesOnly,
				RegexMatchProcessName = RegexMatchProcessName,
				GroupedProcessNames = GroupProcessNames.Select(x => x.Name).ToList()

			};

			service.SaveSettings(settings);
		}

		[RelayCommand]
		public void AddRule()
		{
			GroupingRules.Add(new GroupingRule());
		}

		[RelayCommand]
		public void RemoveRule(GroupingRule rule)
		{
			GroupingRules.Remove(rule);
		}

		[RelayCommand]
		public void AddProcessName()
		{
			GroupProcessNames.Add(new(""));
		}
		[RelayCommand]
		public void RemoveProcessName(ProcessName processName)
		{
			GroupProcessNames.Remove(processName);
		}
	}
	public class ProcessName
	{
		public string Name { get; set; }

		public ProcessName(string name)
		{
			Name = name;
		}
	}
}
