using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IPTVPlayer.Avalonia.Models;
using IPTVPlayer.Avalonia.Services;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVPlayer.Avalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly M3uService _m3uService;
        private readonly SettingsService _settingsService;
        private readonly EpgService _epgService;
        private List<Channel> _allChannels;
        private LibVLC _libVLC;

        [ObservableProperty]
        private MediaPlayer mediaPlayer;

        [ObservableProperty]
        private string? m3uFilePath;

        [ObservableProperty]
        private bool isAutoLoadEnabled;

        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Channels))]
        private string? selectedCategory;

        [ObservableProperty]
        private Channel? selectedChannel;

        public ObservableCollection<Channel> Channels
        {
            get
            {
                var filteredChannels = _allChannels;

                if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "Svi kanali")
                {
                    filteredChannels = filteredChannels.Where(c => c.GroupTitle == SelectedCategory).ToList();
                }

                if (!string.IsNullOrEmpty(FilterText))
                {
                    filteredChannels = filteredChannels.Where(c => c.Name.ToLower().Contains(FilterText.ToLower())).ToList();
                }

                return new ObservableCollection<Channel>(filteredChannels);
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Channels))]
        private string? filterText;

        [ObservableProperty]
        private double fontSize = 14;

        public int Volume
        {
            get => MediaPlayer.Volume;
            set
            {
                if (MediaPlayer.Volume != value)
                {
                    MediaPlayer.Volume = value;
                    OnPropertyChanged();
                    SettingsChanged();
                }
            }
        }

        public Action? ToggleFullScreenAction { get; set; }
        public Action<ThemeVariant>? SetThemeAction { get; set; }

        public MainWindowViewModel()
        {
            _m3uService = new M3uService();
            _settingsService = new SettingsService();
            _epgService = new EpgService();
            _allChannels = new List<Channel>();
            Categories = new ObservableCollection<string>();

            _libVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVLC);

            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();
            M3uFilePath = settings.LastM3uPath;
            Volume = settings.Volume;
            IsAutoLoadEnabled = settings.IsAutoLoadEnabled;
        }

        private void SettingsChanged()
        {
            var settings = new AppSettings
            {
                LastM3uPath = this.M3uFilePath,
                Volume = this.Volume,
                IsAutoLoadEnabled = this.IsAutoLoadEnabled
            };
            _settingsService.SaveSettings(settings);
        }

        [RelayCommand]
        private async Task LoadM3u()
        {
            if (string.IsNullOrWhiteSpace(M3uFilePath)) return;

            _allChannels = await _m3uService.ParseM3u(M3uFilePath);
            SettingsChanged();

            var groupTitles = _allChannels.Select(c => c.GroupTitle).Distinct().OrderBy(g => g).ToList();
            Categories.Clear();
            Categories.Add("Svi kanali");
            foreach (var group in groupTitles)
            {
                Categories.Add(group);
            }

            SelectedCategory = "Svi kanali";
            OnPropertyChanged(nameof(Channels));

            await LoadEpgData();
        }

        private async Task LoadEpgData()
        {
            foreach(var channel in _allChannels)
            {
                channel.CurrentProgram = await _epgService.GetCurrentProgram(channel);
            }
            OnPropertyChanged(nameof(Channels));
        }

        [RelayCommand]
        private async Task SelectFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "M3U datoteke", Extensions = { "m3u", "m3u8" } });
            dialog.AllowMultiple = false;

            var result = await dialog.ShowAsync(new Window());

            if (result != null && result.Any())
            {
                M3uFilePath = result.First();
                await LoadM3u();
            }
        }

        [RelayCommand]
        private void Play(Channel? channel = null)
        {
            var channelToPlay = channel ?? SelectedChannel;
            if (channelToPlay == null) return;
            MediaPlayer.Play(new Media(_libVLC, channelToPlay.Url, FromType.FromLocation));
        }

        [RelayCommand]
        private void Stop()
        {
            MediaPlayer.Stop();
        }

        [RelayCommand]
        private void Pause()
        {
            MediaPlayer.Pause();
        }

        [RelayCommand]
        private void NextChannel()
        {
            if (SelectedChannel == null) return;
            var currentChannels = Channels.ToList();
            var currentIndex = currentChannels.IndexOf(SelectedChannel);
            if (currentIndex < currentChannels.Count - 1)
            {
                SelectedChannel = currentChannels[currentIndex + 1];
            }
        }

        [RelayCommand]
        private void PrevChannel()
        {
            if (SelectedChannel == null) return;
            var currentChannels = Channels.ToList();
            var currentIndex = currentChannels.IndexOf(SelectedChannel);
            if (currentIndex > 0)
            {
                SelectedChannel = currentChannels[currentIndex - 1];
            }
        }

        [RelayCommand]
        private void ToggleFullScreen()
        {
            ToggleFullScreenAction?.Invoke();
        }

        [RelayCommand]
        private void IncreaseFontSize()
        {
            if(FontSize < 24)
                FontSize += 2;
        }

        [RelayCommand]
        private void DecreaseFontSize()
        {
            if(FontSize > 10)
                FontSize -= 2;
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            if (Application.Current is null) return;
            var currentTheme = Application.Current.RequestedThemeVariant;
            var newTheme = currentTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
            SetThemeAction?.Invoke(newTheme);
        }

        partial void OnSelectedChannelChanged(Channel? value)
        {
            Play(value);
        }

        partial void OnM3uFilePathChanged(string? value)
        {
            SettingsChanged();
        }

        partial void OnIsAutoLoadEnabledChanged(bool value)
        {
            SettingsChanged();
        }
    }
}
