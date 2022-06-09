using AvaloniaCallCenter.Models;
using AvaloniaCallCenter.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaCallCenter.ViewModels
{
    public class ClientDetailsViewModel : ReactiveObject, IRoutableViewModel
    {
        public IWindowContainer Container { get; private set; }
        public string? UrlPathSegment => "/clientDetails";
        public IScreen HostScreen { get; }

        private Client _client;

        private Status _buffer_status;

        #region ICommands

        public ICommand OnClickBack { get; private set; }
        public ICommand OnClickSubmitClient { get; private set; }
        public ICommand OnClickDeleteClient { get; private set; }
        public ICommand OnClickAddEvent { get; private set; }
        public ICommand OnClickDeleteEvent { get; private set; }

        #endregion

        #region Values

        //Client
        private string _name;
        private string _email;
        private string _phone;
        private string _city;
        private DateTimeOffset? _date_of_birth;
        private string _description;
        private Status _selected_status;
        private ObservableCollection<Status> _statuses = new ObservableCollection<Status>()
        {
            Status.NEW
        };



        //Event
        private string _event_title;
        private string _event_description;
        private DateTimeOffset? _event_date;
        private TimeSpan? _event_time;
        private Event _selected_event;

        private ObservableCollection<Event> _events = new ObservableCollection<Event>();


        #endregion
        
        
        public ClientDetailsViewModel(IWindowContainer container, Client client, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            Container = container;

            if(client != null)
            {

                _client = client;
                Name = client.Name;
                Phone = client.Phone;
                City = client.City;
                Email = client.Email;
                Description = client.Description;
                DateOfBirth = new DateTimeOffset(client.DateOfBirth);
                SelectedStatus = client.Status;
                _buffer_status = client.Status;

                foreach (var item in _client.Events)
                {
                    Events.Add(item);
                }

                Statuses.Add(Status.NO_ANSWER);
                Statuses.Add(Status.NO_DATE);
                Statuses.Add(Status.CONFIRMED);
                Statuses.Add(Status.TRANSFERRED);
                Statuses.Add(Status.BLOCKED);
            }
            
            #region OnClick

            OnClickBack = ReactiveCommand.Create(() =>
            {
                if(container != null)
                {
                    container.GoBack();
                }
            });

            OnClickSubmitClient = ReactiveCommand.Create(async () =>
            {
                if (_client == null) 
                    _client = new Client() { Events = new List<Event>()};
                

                _client.Name = Name;
                _client.Phone = Phone;
                _client.City = City;
                _client.DateOfBirth = ((DateTimeOffset) DateOfBirth).DateTime;
                _client.Description = Description;
                _client.Email = Email;
                _client.Status = SelectedStatus;


                _client.Events.Clear();
                foreach (Event e in Events)
                {
                    _client.Events.Add(e);
                }

                var result = false;
                if (_client.Id == 0)
                    result = await SignalRConnection.addClientAsync(_client);
                else
                    result = await SignalRConnection.updateClientAsync(_client);

                if (result)
                {
                    if (_client.Id == 0)
                        MainWindowViewModel._clients.Add(_client);

                    if(_buffer_status != _client.Status)
                    {
                        if(client.Status == Status.BLOCKED)
                            MainWindowViewModel._user.countBlocked++;
                        if(client.Status == Status.TRANSFERRED)
                            MainWindowViewModel._user.countTransferred++;

                        await SignalRConnection.updateUserCountFieldsAsync(MainWindowViewModel._user);
                    }
                    container.GoBack();
                }
            });

            OnClickDeleteClient = ReactiveCommand.Create(async () =>
            {
                if(_client != null)
                {
                    var result = await SignalRConnection.deleteClientAsync(_client.Id);
                    if (result)
                    {
                        MainWindowViewModel._clients.Remove(_client);
                        container.GoBack();
                    }
                }
            });

            OnClickAddEvent = ReactiveCommand.Create(async () =>
            {
                DateTime date = ((DateTimeOffset)SelectedEventDate).DateTime;
                TimeSpan time = ((TimeSpan)SelectedEventTime);
                DateTime EventDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
                Event @event = new Event
                {
                    Title = EventTitle,
                    Description = EventDescription,
                    EventDateTime = EventDateTime,
                };
                Events.Add(@event);

                if (_client != null)
                {
                    await SignalRConnection.addEventAsync(_client.Id, @event);
                }
            });


            OnClickDeleteEvent = ReactiveCommand.Create(async () =>
            {
                if (_client != null && SelectedEvent.Id != 0)
                {
                    bool result = await SignalRConnection.deleteEventAsync(_client.Id, SelectedEvent.Id);
                    if (result)
                    {
                        Events.Remove(SelectedEvent);
                    }
                }else if(SelectedEvent != null)
                {
                    Events.Remove(SelectedEvent);
                }
            });

            #endregion
        }


        #region ValuesNotify

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => this.RaiseAndSetIfChanged(ref _phone, value);
        }

        public string City
        {
            get => _city;
            set => this.RaiseAndSetIfChanged(ref _city, value);
        }

        public DateTimeOffset? DateOfBirth
        {
            get => _date_of_birth;
            set => this.RaiseAndSetIfChanged(ref _date_of_birth, value);
        }


        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public Status SelectedStatus
        {
            get => _selected_status;
            set => this.RaiseAndSetIfChanged(ref _selected_status, value);
        }

        public ObservableCollection<Status> Statuses
        {
            get => _statuses;
            set => this.RaiseAndSetIfChanged(ref _statuses, value);
        }

        public string EventTitle
        {
            get => _event_title;
            set => this.RaiseAndSetIfChanged(ref _event_title, value);
        }

        public string EventDescription
        {
            get => _event_description;
            set => this.RaiseAndSetIfChanged(ref _event_description, value);
        }

        public DateTimeOffset? SelectedEventDate
        {
            get => _event_date;
            set => this.RaiseAndSetIfChanged(ref _event_date, value);
        }

        public TimeSpan? SelectedEventTime
        {
            get => _event_time;
            set => this.RaiseAndSetIfChanged(ref _event_time, value);
        }

        public ObservableCollection<Event> Events
        {
            get => _events;
            set => this.RaiseAndSetIfChanged(ref _events, value);
        }

        public Event SelectedEvent
        {
            get => _selected_event;
            set => this.RaiseAndSetIfChanged(ref _selected_event, value);
        }

        #endregion
    }
}
