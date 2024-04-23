using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotSights.Core.Common.Types;
using DotSights.Dashboard.Models;
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

		partial void OnRunOnStartupChanged(bool oldValue, bool newValue)
		{
			if (newValue)
			{
				StartupService.SetStartup();
			}
			else
			{
				StartupService.RemoveStartup();
			}
		}

		public ObservableCollection<GroupingRule> GroupingRules { get => _groupingRules; set => this.SetProperty(ref _groupingRules, value); }
		private ObservableCollection<GroupingRule> _groupingRules = new();


		public SettingsPageViewModel()
		{
			var service = ConfigurationService.Instance;
			var settings = service.LoadSettings();
			GroupItemsWithSameProcessName = settings.GroupItemsWithSameProcessName;
			GroupItemsUsingGroupingRules = settings.GroupItemsUsingGroupingRules;
			GroupingRules = new(settings.GroupingRules);
			TrackerSaveDelay = settings.TrackerSaveInterval;
			RunOnStartup = StartupService.IsStartup();
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
				TrackerSaveInterval = TrackerSaveDelay
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
	}
}
