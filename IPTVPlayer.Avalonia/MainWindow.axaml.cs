using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using IPTVPlayer.Avalonia.ViewModels;
using LibVLCSharp.Avalonia;
using System;
using System.Diagnostics;

namespace IPTVPlayer.Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        private DispatcherTimer _hideControlsTimer;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _viewModel.ToggleFullScreenAction = ToggleFullScreen;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == WindowStateProperty && WindowState == WindowState.Maximized)
            {
                // Un-fullscreen if the user maximizes the window manually
                if (_viewModel.IsVideoFullScreen)
                {
                    _viewModel.IsVideoFullScreen = false;
                }
            };

            this.Loaded += MainWindow_Loaded;
            this.Closing += (s, e) => _viewModel.Dispose();
            this.KeyDown += OnKeyDown;

            _hideControlsTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _hideControlsTimer.Tick += (s, e) =>
            {
                this.FindControl<StackPanel>("playbackControls").IsVisible = false;
                _hideControlsTimer.Stop();
            };

            this.PointerMoved += (s, e) =>
            {
                if (WindowState == WindowState.FullScreen)
                {
                    this.FindControl<StackPanel>("playbackControls").IsVisible = true;
                    _hideControlsTimer.Stop();
                    _hideControlsTimer.Start();
                }
            };
        }

        private void ToggleFullScreen()
        {
            var playbackControls = this.FindControl<StackPanel>("playbackControls");
            if (WindowState == WindowState.FullScreen)
            {
                WindowState = WindowState.Maximized;
                SystemDecorations = SystemDecorations.Full;
                playbackControls.IsVisible = true;
                _hideControlsTimer.Stop();
                Cursor = new Cursor(StandardCursorType.Arrow);
            }
            else
            {
                WindowState = WindowState.FullScreen;
                SystemDecorations = SystemDecorations.None;
                _hideControlsTimer.Start();
                Cursor = new Cursor(StandardCursorType.None);
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && WindowState == WindowState.FullScreen)
            {
                ToggleFullScreen();
            }
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            var videoView = this.FindControl<VideoView>("videoView");
            if (videoView != null)
            {
                videoView.MediaPlayer = _viewModel.MediaPlayer;
            }

            if (_viewModel.IsAutoLoadEnabled && _viewModel.LoadM3uCommand.CanExecute(null))
            {
                _viewModel.LoadM3uCommand.Execute(null);
            }
        }
    }
}
