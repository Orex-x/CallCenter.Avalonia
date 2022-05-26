using ReactiveUI;
using System;
using System.Collections.Generic;
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

        public MainWindowViewModel()
        {
            OnClick = ReactiveCommand.Create(() =>
            {
                Greeting = "hello dasha";
            });
        }
    }
}
