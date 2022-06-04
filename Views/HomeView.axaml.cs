using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaCallCenter.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace AvaloniaCallCenter.Views
{
    public partial class HomeView : ReactiveUserControl<HomeViewModel>
    {
        public HomeView()
        {
            // ����� WhenActivated ������������ ��� ���������� ���������� 
            // ���� � ������ ��������� � ����������� ������ �������������.
            this.WhenActivated((CompositeDisposable disposable) => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}