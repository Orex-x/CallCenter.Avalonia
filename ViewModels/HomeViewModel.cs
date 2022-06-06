using AvaloniaCallCenter.Models;
using AvaloniaCallCenter.Services;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace AvaloniaCallCenter.ViewModels
{
    [DataContract]
    public class HomeViewModel : ReactiveObject, IRoutableViewModel
    {
        public IWindowContainer Container { get; private set; }


        #region ICommand
        public ICommand OnClickConnect { get; private set; }
        public ICommand OnClickSendMessage { get; private set; }
        public ICommand OnClickGetAllCalls { get; private set; }
        public ICommand OnClickAddContact { get; private set; }
        public ICommand OnClickCall { get; private set; }
        public ICommand OnClickLogout { get; private set; }
        public ICommand OnClickClientDetails { get; private set; }
        public ICommand OnClickClientAdd { get; private set; }

        #endregion

        #region ValuesAccount

        private string _account_name;
        private string _account_surname;
        private string _account_lastname;
        private string _account_login;
        
        #endregion

        #region Values

        HubConnection connection;
        private string _title = "Authorization";

        
        private string _message;
        private string _contact_name;
        private string _contact_phone;
        private Client _selected_client;

        private ObservableCollection<Message> _myItems = new ObservableCollection<Message>();
        private ObservableCollection<Connection> _connections = new ObservableCollection<Connection>();
        private ObservableCollection<Client> _clients = new ObservableCollection<Client>()
        {
            new Client{ Name = "Олег", Phone = "88005557777(8)"}
        };

        #endregion

        #region ValuesNotify
        public string Greeting
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }


        public string ContactName
        {
            get => _contact_name;
            set => this.RaiseAndSetIfChanged(ref _contact_name, value);
        }

        public string ContactPhone
        {
            get => _contact_phone;
            set => this.RaiseAndSetIfChanged(ref _contact_phone, value);
        }

        public string AccountName
        {
            get => _account_name;
            set => this.RaiseAndSetIfChanged(ref _account_name, value);
        }


        public string AccountSurname
        {
            get => _account_surname;
            set => this.RaiseAndSetIfChanged(ref _account_surname, value);
        }


        public string AccountLastname
        {
            get => _account_lastname;
            set => this.RaiseAndSetIfChanged(ref _account_lastname, value);
        }

        public string AccountLogin
        {
            get => _account_login;
            set => this.RaiseAndSetIfChanged(ref _account_login, value);
        }

        public Client SelectedClient
        {
            get => _selected_client;
            set => this.RaiseAndSetIfChanged(ref _selected_client, value);
        }

        public ObservableCollection<Message> MyItems
        {
            get => _myItems;
            set => this.RaiseAndSetIfChanged(ref _myItems, value);
        }

        public ObservableCollection<Connection> Connections
        {
            get => _connections;
            set => this.RaiseAndSetIfChanged(ref _connections, value);
        }

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }
        #endregion


        public HomeViewModel(IWindowContainer container, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Container = container;
            var user = SignalRConnection.getUser();

            var clients = SignalRConnection.getAllClients();
            foreach (var client in clients)
                Clients.Add(client);
            
            AccountLogin = user.login;
            AccountName = user.name;
            
            OnClickConnect = ReactiveCommand.Create(() =>
            {
                ConectionClick();
            });

            OnClickSendMessage = ReactiveCommand.Create(() =>
            {
                SendMessageClick();
            });

            OnClickGetAllCalls = ReactiveCommand.Create(() =>
            {
                GetAllCalls();
            });

            OnClickAddContact = ReactiveCommand.Create(() =>
            {
                Clients.Add(new Client { Name = ContactName, Phone = ContactPhone });
            });

            OnClickClientDetails = ReactiveCommand.Create(() =>
            {
                if(container != null && SelectedClient != null)
                {
                    container.GoToClientDetails(SelectedClient);
                }
            });

            OnClickClientAdd = ReactiveCommand.Create(() =>
            {
                if (container != null)
                {
                    container.GoToClientDetails(null);
                }
            });

            OnClickLogout = ReactiveCommand.Create(() =>
            {
                if(container != null)
                {
                    SignalRConnection.setUserAndConnectionNull();
                    container.GoToAuthorization();
                }
            });

            OnClickCall = ReactiveCommand.Create(() =>
            {
                try
                {
                    connection.InvokeAsync("CallPhone", SelectedClient.Phone);
                }
                catch (Exception ex)
                {
                    MyItems.Add(new Message { Title = ex.Message });
                }
            });
        }

        #region MethodsClick
        private async void SendMessageClick()
        {
            try
            {
                await connection.InvokeAsync("CallPhone", Message);
            }
            catch (Exception ex)
            {
                MyItems.Add(new Message { Title = ex.Message });
            }
        }

        private async void ConectionClick()
        {
            connection = SignalRConnection.GetConnection();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var newMessage = $"{user}: {message}";
                MyItems.Add(new Message { Title = newMessage });

            });

            connection.On<List<Call>>("ReceiveAllCalls", (calls) =>
            {
                foreach (var call in calls)
                {
                    var item = $"Human: {call.Name}\n Number: {call.Number}";
                    MyItems.Add(new Message { Title = item });
                }
            });

            connection.On<string>("ReceiveMeMessage", (message) =>
            {
                var newMessage = $"{message}";
                MyItems.Add(new Message { Title = message });

            });

            connection.On<Connection>("ReceiveConnected", (connection) =>
            {
                Connections.Add(connection);
            });

            connection.On<Connection>("ReceiveDisconnected", (connection) =>
            {
                foreach (var item in Connections)
                {
                    if (item.connectionID == connection.connectionID)
                        Connections.Remove(item);
                }
            });

            try
            {
                await connection.StartAsync();
                MyItems.Add(new Message { Title = "Connection started" });
            }
            catch (Exception ex)
            {
                MyItems.Add(new Message { Title = ex.Message });
            }
        }


        private async void GetAllCalls()
        {
            try
            {
                await connection.InvokeAsync("GetAllCalls");
            }
            catch (Exception ex)
            {
                MyItems.Add(new Message { Title = ex.Message });
            }
        }

        #endregion

        public string? UrlPathSegment => "/home";

        public IScreen HostScreen { get; }
    }
}
