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
        public ICommand TerminateConnection { get; private set; }
        public ICommand TerminateAllConnection { get; private set; }
        public ICommand UpdateAccountUnfo { get; private set; }

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
        private Connection _selected_seesion;
        private User _user;

        private string _count_calls;
        private string _count_blocked;
        private string _count_transferred;

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        private ObservableCollection<Call> _call_history = new ObservableCollection<Call>()
        {
            new Call() { Date = "12/12 11:23", Name = "Unknown", Number = "88005553535"}
        };
        private ObservableCollection<Connection> _connections = new ObservableCollection<Connection>();
        

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
        
        public string CountCalls
        {
            get => _count_calls;
            set => this.RaiseAndSetIfChanged(ref _count_calls, value);
        } 
        
        public string CountBlocked
        {
            get => _count_blocked;
            set => this.RaiseAndSetIfChanged(ref _count_blocked, value);
        }
        public string CountTransferred
        {
            get => _count_transferred;
            set => this.RaiseAndSetIfChanged(ref _count_transferred, value);
        }


        public User MainUser
        {
            get => MainWindowViewModel._user;
            set => this.RaiseAndSetIfChanged(ref MainWindowViewModel._user, value);
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
        
        public Connection SelectedConnection
        {
            get => _selected_seesion;
            set => this.RaiseAndSetIfChanged(ref _selected_seesion, value);
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
            get => MainWindowViewModel._clients;
            set => this.RaiseAndSetIfChanged(ref MainWindowViewModel._clients, value);
        }

        public ObservableCollection<Call> CallHistory
        {
            get => _call_history;
            set => this.RaiseAndSetIfChanged(ref _call_history, value);
        }
        #endregion

         
        public async Task getData()
        {
            MainUser = await SignalRConnection.getUserAsync();

            CountCalls = "Звонков: " + MainUser.countCalls;
            CountBlocked = "Заблокировано: " + MainUser.countBlocked;
            CountTransferred = "Отправлено: " + MainUser.countTransferred;

            var clients = await SignalRConnection.getAllClientsAsync();
            MainWindowViewModel._clients.Clear();
            foreach (var client in clients)
                MainWindowViewModel._clients.Add(client);

            
        }


        public HomeViewModel(IWindowContainer container, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Container = container;

            ConectionClick();

            getData();


            UpdateAccountUnfo = ReactiveCommand.Create(async () =>
            {
                bool ok = await SignalRConnection.UpdateUser(MainUser);
            });

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


            TerminateConnection = ReactiveCommand.Create(() =>
            {
                if (SelectedConnection != null)
                {
                    connection.InvokeAsync("TerminateConnection", SelectedConnection);
                }
            });


            TerminateAllConnection = ReactiveCommand.Create(() =>
            {
                connection.InvokeAsync("TerminateAllConnection");
            });


            OnClickLogout = ReactiveCommand.Create(() =>
            {
                LogOut();
            });

            OnClickCall = ReactiveCommand.Create(async () =>
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
            if(connection == null)
            {
                connection = SignalRConnection.GetConnection();

                connection.On<Message>("ReceiveMessage", (message) =>
                {
                    Messages.Add(message);
                });

                connection.On("ReceiveTerminateConnection", () =>
                {
                    LogOut();
                });

                connection.On<List<Call>>("ReceiveAllCalls", (calls) =>
                {
                    foreach (var call in calls)
                    {
                        var item = $"Human: {call.Name}\n Number: {call.Number}";
                        Messages.Add(new Message { Title = item });
                    }
                });

                connection.On<Call>("ReceiveCallLog", async (call) =>
                {
                    MainWindowViewModel._user.countCalls++;
                    CountCalls = "Звонков: " + MainUser.countCalls;
                    await SignalRConnection.updateUserCountFieldsAsync(MainUser);

                    CallHistory.Add(call);
                });

                connection.On<string>("ReceiveMeMessage", (message) =>
                {
                    var newMessage = $"{message}";
                    Messages.Add(new Message { Title = message });

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
                    Messages.Add(new Message { Title = "Connection started" });
                }
                catch (Exception ex)
                {
                    Messages.Add(new Message { Title = ex.Message });
                }
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


        public void LogOut()
        {
            if (Container != null)
            {
                SignalRConnection.setUserAndConnectionNull();
                Container.GoToAuthorization();
            }
        }
        #endregion

        public string? UrlPathSegment => "/home";

        public IScreen HostScreen { get; }
    }
}
