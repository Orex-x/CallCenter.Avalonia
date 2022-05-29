using AvaloniaCallCenter.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace AvaloniaCallCenter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand OnClick { get; private set; }

        private string _title = "hello Avalonia";
        public string Greeting {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }


        private ObservableCollection<Message> _myItems = new ObservableCollection<Message>
        {
            new Message{ Title = "hui 1" },
            new Message{ Title = "hui 2" },
            new Message{ Title = "hui 3" }
        };
        public ObservableCollection<Message> MyItems
        {
            get => _myItems;
            set => this.RaiseAndSetIfChanged(ref _myItems, value);
        }



        public MainWindowViewModel()
        {
            OnClick = ReactiveCommand.Create(() =>
            {
                Greeting = "hello dasha";
            });

           
        }
    }
}
