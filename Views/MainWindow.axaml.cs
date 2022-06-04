using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaCallCenter.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;

using System.Windows.Input;
using static AvaloniaCallCenter.Views.MainWindow;

namespace AvaloniaCallCenter.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
