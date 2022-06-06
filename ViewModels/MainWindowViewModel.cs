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
        public void GoToRegistartion();
        public void GoToAuthorization();
        public void GoToClientDetails(Client client);
        public void GoBack();
       
    }

   

    [DataContract]
    public class MainWindowViewModel : ReactiveObject, IScreen, IWindowContainer
    {
        private RoutingState _router = new RoutingState();


        public MainWindowViewModel()
        {
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
            Router.Navigate.Execute(new HomeViewModel(this));
        }

        public void GoToRegistartion()
        {
            Router.Navigate.Execute(new RegistrationViewModel(this));
        }

        public void GoToAuthorization()
        {
            Router.Navigate.Execute(new AuthorizationWindowViewModel(this));
        }

        public void GoToClientDetails(Client client)
        {
            Router.Navigate.Execute(new ClientDetailsViewModel(this, client));
        }

        void IWindowContainer.GoBack()
        {
            Router.NavigateBack.Execute();
        }
    }

   
}
