using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using IPTVPlayer.Avalonia.ViewModels;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;

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
