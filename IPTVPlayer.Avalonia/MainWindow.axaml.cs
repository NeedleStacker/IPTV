using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using IPTVPlayer.Avalonia.ViewModels;
using LibVLCSharp.Avalonia;
using System.Diagnostics;

namespace IPTVPlayer.Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _viewModel.ToggleFullScreenAction = () =>
            {
                WindowState = WindowState == WindowState.FullScreen ? WindowState.Maximized : WindowState.FullScreen;
            };

            _viewModel.SetThemeAction = (theme) =>
            {
                if (Application.Current != null)
                {
                    Application.Current.RequestedThemeVariant = theme;
                }
            };

            this.Loaded += MainWindow_Loaded;
            this.Closing += (s, e) => _viewModel.Dispose();
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[MainWindow] Window Loaded event fired.");
            var videoView = this.FindControl<VideoView>("videoView");
            if (videoView != null)
            {
                videoView.MediaPlayer = _viewModel.MediaPlayer;
            }

            if (_viewModel.IsAutoLoadEnabled && _viewModel.LoadM3uCommand.CanExecute(null))
            {
                Debug.WriteLine("[MainWindow] Auto-load is enabled. Executing LoadM3uCommand.");
                _viewModel.LoadM3uCommand.Execute(null);
            }
            else
            {
                Debug.WriteLine("[MainWindow] Auto-load is disabled or command cannot execute.");
            }
        }
    }
}
