using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace DotSights.Dashboard.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase? _selectedViewModel = new HomePageViewModel();

        [ObservableProperty]
        public ListItemTemplate? _selectedListItem;

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null)
                return;
            var instance = Activator.CreateInstance(value.Type);
            if (instance is null)
                return;
            SelectedViewModel = (ViewModelBase)instance;
        }

        public ObservableCollection<ListItemTemplate> Items { get; } = new ObservableCollection<ListItemTemplate>
        {
            new ListItemTemplate("Home", typeof(HomePageViewModel), "home_regular"),
            new ListItemTemplate("Settings", typeof(SettingsPageViewModel), "home_regular"),
        };
        [RelayCommand]
        private void TriggerPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }
    }

    public class ListItemTemplate
    {
        public StreamGeometry Icon { get; set; }
        public string Label { get; set; }
        public Type Type { get; set; }

        public ListItemTemplate(string label, Type type, string iconKey)
        {
            Label = label;
            Type = type;
            Application.Current!.TryFindResource(iconKey, out var res);
            Icon = (StreamGeometry)res!;
        }
    }
}
