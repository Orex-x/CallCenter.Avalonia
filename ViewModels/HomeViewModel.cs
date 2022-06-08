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
        public ICommand OnClickSendMessage { get; private set; }
        public ICommand OnClickGetAllCalls { get; private set; }
        public ICommand OnClickCall { get; private set; }
        public ICommand OnClickLogout { get; private set; }
        public ICommand OnClickClientDetails { get; private set; }
        public ICommand OnClickClientAdd { get; private set; }
        public ICommand OnClickSelectedCall { get; private set; }

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
        private Client _selected_client;
        private Call _selected_call;

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        private ObservableCollection<Call> _call_history = new ObservableCollection<Call>()
        {
            new Call() { Date = "12/12 11:23", Name = "Unknown", Number = "88005553535"}
        };
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
        
        public Call SelectedCall
        {
            get => _selected_call;
            set => this.RaiseAndSetIfChanged(ref _selected_call, value);
        }

        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
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

        public ObservableCollection<Call> CallHistory
        {
            get => _call_history;
            set => this.RaiseAndSetIfChanged(ref _call_history, value);
        }
        #endregion

        public async Task getData()
        {
            var user = await SignalRConnection.getUserAsync();

            var clients = await SignalRConnection.getAllClientsAsync();
            foreach (var client in clients)
                Clients.Add(client);

            AccountLogin = user.login;
            AccountName = user.name;
        }


        public HomeViewModel(IWindowContainer container, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Container = container;

            ConectionClick();


      

            OnClickSendMessage = ReactiveCommand.Create(() =>
            {
                SendMessageClick();
            });

            OnClickGetAllCalls = ReactiveCommand.Create(() =>
            {
                GetAllCalls();
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

            OnClickSelectedCall = ReactiveCommand.Create(() =>
            {
                if(SelectedCall != null)
                {
                    connection.InvokeAsync("CallPhone", SelectedCall.Number);
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
                    Messages.Add(new Message { Title = ex.Message });
                }
            });

            getData();
        }

        #region MethodsClick
        private async void SendMessageClick()
        {
            try
            {
                await connection.InvokeAsync("SendMessage", new Message() { Title = Message, Created = DateTime.Now});
            }
            catch (Exception ex)
            {
                Messages.Add(new Message { Title = ex.Message });
            }
        }

        private async void ConectionClick()
        {
            connection = SignalRConnection.GetConnection();

            connection.On<Message>("ReceiveMessage", (message) =>
            {
                Messages.Add(message);
            });

            connection.On<List<Call>>("ReceiveAllCalls", (calls) =>
            {
                foreach (var call in calls)
                {
                    var item = $"Human: {call.Name}\n Number: {call.Number}";
                    Messages.Add(new Message { Title = item });
                }
            });

            connection.On<Call>("ReceiveCallLog", (call) =>
            {
                CallHistory.Add(call);
            });

            connection.On<string>("ReceiveMeMessage", (message) =>
            {
                var newMessage = $"{message}";
                Messages.Add(new Message { Title = message });

            });

            connection.On<string>("ReceiveConnected", (connection) =>
            {
                Connections.Add(new Connection { connectionID = connection});
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
                Messages.Add(new Message { Title = "Connection started" });
            }
            catch (Exception ex)
            {
                Messages.Add(new Message { Title = ex.Message });
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
                Messages.Add(new Message { Title = ex.Message });
            }
        }

        #endregion

        public string? UrlPathSegment => "/home";

        public IScreen HostScreen { get; }
    }
}
