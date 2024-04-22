﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotSights.Core.Common.Types;
using DotSights.Dashboard.Models;
using DotSights.Dashboard.Services;
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

		public ObservableCollection<GroupingRule> GroupingRules { get => _groupingRules; set => this.SetProperty(ref _groupingRules, value); }
		private ObservableCollection<GroupingRule> _groupingRules = new();


		public SettingsPageViewModel()
		{
			var service = ConfigurationService.Instance;
			var settings = service.LoadSettings();
			GroupItemsWithSameProcessName = settings.GroupItemsWithSameProcessName;
			GroupItemsUsingGroupingRules = settings.GroupItemsUsingGroupingRules;
			GroupingRules = new(settings.GroupingRules);
		}

		[RelayCommand]
		public void SaveSettings()
		{
			var service = ConfigurationService.Instance;
			var settings = new DashboardSettings
			{
				GroupItemsWithSameProcessName = GroupItemsWithSameProcessName,
				GroupItemsUsingGroupingRules = GroupItemsUsingGroupingRules,
				GroupingRules = GroupingRules.ToList()
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