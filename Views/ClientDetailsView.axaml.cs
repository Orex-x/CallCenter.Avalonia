using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaCallCenter.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace AvaloniaCallCenter.Views
{
    public partial class ClientDetailsView : ReactiveUserControl<ClientDetailsViewModel>
    {
        public ClientDetailsView()
        {
            // ¬ызов WhenActivated используетс€ дл€ выполнени€ некоторого 
            // кода в момент активации и деактивации модели представлени€.
            this.WhenActivated((CompositeDisposable disposable) => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
