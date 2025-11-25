using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
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
    public partial class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly M3uService _m3uService = new();
        private readonly SettingsService _settingsService = new();
        private readonly EpgService _epgService = new();
        private List<Channel> _allChannels = new();
        private readonly LibVLC _libVLC = new();

        [ObservableProperty]
        private MediaPlayer mediaPlayer;

        [ObservableProperty]
        private string? m3uFilePath;

        [ObservableProperty]
        private bool isAutoLoadEnabled;

        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveCategoryUpCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveCategoryDownCommand))]
        private string? selectedCategory;

        [ObservableProperty]
        private Channel? selectedChannel;

        [ObservableProperty]
        private string? filterText;

        public ObservableCollection<Channel> Channels { get; }

        [ObservableProperty]
        private double fontSize = 18;

        public bool IsVOD => MediaPlayer?.Length > 0;

        [ObservableProperty]
        private float position;

        [ObservableProperty]
        private double buttonScale = 1.0;

        [ObservableProperty]
        private double buttonFontSize = 18;

        [ObservableProperty]
        private Thickness buttonPadding = new(15, 10);

        [ObservableProperty]
        private bool isVideoFullScreen;

        [ObservableProperty]
        private WindowState windowState = WindowState.Maximized;

        [ObservableProperty]
        private SystemDecorations systemDecorations = SystemDecorations.Full;

        [ObservableProperty]
        private int volume;

        public Action? ToggleFullScreenAction { get; set; }
        public Action<ThemeVariant>? SetThemeAction { get; set; }

        public MainWindowViewModel()
        {
            Categories = new();
            Channels = new();

            MediaPlayer = new(_libVLC);

            MediaPlayer.LengthChanged += (s, e) => OnPropertyChanged(nameof(IsVOD));
            MediaPlayer.PositionChanged += (s, e) => Position = e.Position;

            LoadSettings();
        }

        partial void OnPositionChanged(float value)
        {
            if (Math.Abs(MediaPlayer.Position - value) > 0.01)
            {
                MediaPlayer.Position = value;
            }
        }

        partial void OnButtonScaleChanged(double value)
        {
            ButtonFontSize = 18 * value;
            ButtonPadding = new(15 * value, 10 * value);
            SettingsChanged();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();
            M3uFilePath = settings.LastM3uPath;
            Volume = settings.Volume;
            IsAutoLoadEnabled = settings.IsAutoLoadEnabled;
            ButtonScale = settings.ButtonScale > 1 ? settings.ButtonScale : 1;
        }

        private void SettingsChanged()
        {
            var settings = _settingsService.LoadSettings(); // Load existing to preserve other settings
            settings.LastM3uPath = M3uFilePath;
            settings.Volume = Volume;
            settings.IsAutoLoadEnabled = IsAutoLoadEnabled;
            settings.ButtonScale = ButtonScale;
            settings.CategoryOrder = new(Categories); // Save current order
            _settingsService.SaveSettings(settings);
        }

        [RelayCommand]
        private async Task LoadM3u()
        {
            if (string.IsNullOrWhiteSpace(M3uFilePath)) return;

            _allChannels = await _m3uService.ParseM3u(M3uFilePath);

            var settings = _settingsService.LoadSettings();
            var savedOrder = settings.CategoryOrder;

            var groupTitles = _allChannels.Select(c => c.GroupTitle).Distinct().ToList();
            var sortedCategories = groupTitles
                .OrderBy(c => savedOrder.IndexOf(c ?? string.Empty) == -1 ? int.MaxValue : savedOrder.IndexOf(c ?? string.Empty))
                .ThenBy(c => c)
                .ToList();

            Categories.Clear();
            Categories.Add("Svi kanali");
            foreach (var group in sortedCategories)
            {
                if (group != null) Categories.Add(group);
            }

            SelectedCategory = "Svi kanali";
            SettingsChanged(); // Save the potentially updated category list
            UpdateFilteredChannels();

            await LoadEpgData();
        }

        private void UpdateFilteredChannels()
        {
            var filteredChannels = _allChannels;

            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "Svi kanali")
            {
                if (filteredChannels != null)
                    filteredChannels = filteredChannels.Where(c => c.GroupTitle == SelectedCategory).ToList();
            }

            if (!string.IsNullOrEmpty(FilterText))
            {
                if (filteredChannels != null)
                    filteredChannels = filteredChannels.Where(c => c.Name?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            Dispatcher.UIThread.Post(() =>
            {
                Channels.Clear();
                if (filteredChannels != null)
                {
                    foreach(var channel in filteredChannels)
                    {
                        Channels.Add(channel);
                    }
                }
            });
        }

        private async Task LoadEpgData()
        {
            if(_allChannels == null) return;
            foreach(var channel in _allChannels)
            {
                channel.CurrentProgram = await _epgService.GetCurrentProgram(channel);
            }
            UpdateFilteredChannels();
        }

        [RelayCommand]
        private async Task SelectFile()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow is null)
            {
                return;
            }

            var files = await desktop.MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Odaberi M3U datoteku",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("M3U datoteke") { Patterns = new[] { "*.m3u", "*.m3u8" } } }
            });

            if (files.Count > 0)
            {
                var path = files[0].TryGetLocalPath();
                if (path is null) return;

                M3uFilePath = path;
                await LoadM3u();
            }
        }

        [RelayCommand]
        private void Play(Channel? channel = null)
        {
            var channelToPlay = channel ?? SelectedChannel;
            if (channelToPlay?.Url == null) return;

            if (Uri.TryCreate(channelToPlay.Url, UriKind.Absolute, out var uri))
            {
                MediaPlayer.Play(new Media(_libVLC, uri));
            }
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

        [RelayCommand(CanExecute = nameof(CanMoveCategoryUp))]
        private void MoveCategoryUp()
        {
            if (SelectedCategory == null) return;
            var index = Categories.IndexOf(SelectedCategory);
            Categories.Move(index, index - 1);
            SelectedCategory = Categories[index - 1]; // Keep the item selected
            SettingsChanged();
        }

        private bool CanMoveCategoryUp()
        {
            if (SelectedCategory == null || SelectedCategory == "Svi kanali") return false;
            return Categories.IndexOf(SelectedCategory) > 1; // Cannot move above "Svi kanali"
        }

        [RelayCommand(CanExecute = nameof(CanMoveCategoryDown))]
        private void MoveCategoryDown()
        {
            if (SelectedCategory == null) return;
            var index = Categories.IndexOf(SelectedCategory);
            Categories.Move(index, index + 1);
            SelectedCategory = Categories[index + 1]; // Keep the item selected
            SettingsChanged();
        }

        private bool CanMoveCategoryDown()
        {
            if (SelectedCategory == null || SelectedCategory == "Svi kanali") return false;
            return Categories.IndexOf(SelectedCategory) < Categories.Count - 1;
        }

        partial void OnVolumeChanged(int value)
        {
            MediaPlayer.Volume = value;
            SettingsChanged();
        }

        [RelayCommand]
        private void IncreaseVolume()
        {
            Volume = Math.Min(100, Volume + 5);
        }

        [RelayCommand]
        private void DecreaseVolume()
        {
            Volume = Math.Max(0, Volume - 5);
        }

        [RelayCommand]
        private void ToggleFullScreen()
        {
            IsVideoFullScreen = !IsVideoFullScreen;
        }

        partial void OnIsVideoFullScreenChanged(bool value)
        {
            WindowState = value ? WindowState.FullScreen : WindowState.Maximized;
            SystemDecorations = value ? SystemDecorations.None : SystemDecorations.Full;
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

        partial void OnSelectedCategoryChanged(string? value)
        {
            if (value != null)
            {
                UpdateFilteredChannels();
            }
        }

        partial void OnFilterTextChanged(string? value)
        {
            UpdateFilteredChannels();
        }

        partial void OnM3uFilePathChanged(string? value)
        {
            SettingsChanged();
        }

        partial void OnIsAutoLoadEnabledChanged(bool value)
        {
            SettingsChanged();
        }

        public void Dispose()
        {
            MediaPlayer?.Dispose();
            _libVLC?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
