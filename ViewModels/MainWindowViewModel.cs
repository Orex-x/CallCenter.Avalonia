using Avalonia;
using Avalonia.Controls;
using AvaloniaCallCenter.Models;
using AvaloniaCallCenter.Services;
using AvaloniaCallCenter.Views;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using static AvaloniaCallCenter.Views.MainWindow;

namespace AvaloniaCallCenter.ViewModels
{
    public interface IWindowContainer
    {
        public void GoToHome();
       
    }


    [DataContract]
    public class MainWindowViewModel : ReactiveObject, IScreen, IWindowContainer
    {
        private RoutingState _router = new RoutingState();

        public MainWindowViewModel()
        {

/*            var canLogin = this
           .WhenAnyObservable(x => x.Router.CurrentViewModel)
           .Select(current => !(current is AuthorizationWindowViewModel));*/

            Router.Navigate.Execute(new AuthorizationWindowViewModel(this));
        }

        [DataMember]
        public RoutingState Router
        {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }

        public void GoToHome()
        {
            Router.Navigate.Execute(new HomeViewModel());
        }
    }

   
}
