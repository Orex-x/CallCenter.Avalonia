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
        #region ICommand
        public ICommand OnClickConnect { get; private set; }
        public ICommand OnClickSendMessage { get; private set; }
        public ICommand OnClickGetAllCalls { get; private set; }
        public ICommand OnClickAddContact { get; private set; }
        public ICommand OnClickCall { get; private set; }

        #endregion

        #region Values

        HubConnection connection;
        private string _title = "Authorization";
        private string _message;
        private string _contact_name;
        private string _contact_phone;
        private Contact _selected_contact;

        private ObservableCollection<Message> _myItems = new ObservableCollection<Message>();
        private ObservableCollection<Connection> _connections = new ObservableCollection<Connection>();
        private ObservableCollection<Contact> _contacts = new ObservableCollection<Contact>()
        {
            new Contact{ Name = "NAME", Phone = "PHONE"}
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

        public Contact SelectedContact
        {
            get => _selected_contact;
            set => this.RaiseAndSetIfChanged(ref _selected_contact, value);
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

        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set => this.RaiseAndSetIfChanged(ref _contacts, value);
        }

        #endregion


        public HomeViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

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
                Contacts.Add(new Contact { Name = ContactName, Phone = ContactPhone });
            });

            OnClickCall = ReactiveCommand.Create(() =>
            {
                try
                {
                    connection.InvokeAsync("CallPhone", _selected_contact.Phone);
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

            connection.On<List<Calls>>("ReceiveAllCalls", (calls) =>
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
