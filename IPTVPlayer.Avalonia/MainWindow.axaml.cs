using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using IPTVPlayer.Avalonia.ViewModels;
using LibVLCSharp.Avalonia;
using System.ComponentModel;

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

            this.KeyDown += MainWindow_KeyDown;
            this.Loaded += MainWindow_Loaded;
            this.Closing += (s, e) => _viewModel.Dispose();
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _viewModel.IsVideoFullScreen)
            {
                _viewModel.IsVideoFullScreen = false;
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
