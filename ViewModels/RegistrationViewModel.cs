using AvaloniaCallCenter.Models;
using AvaloniaCallCenter.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaCallCenter.ViewModels
{
    public class RegistrationViewModel : ReactiveObject, IRoutableViewModel
    {


        #region ICommand
        public ICommand OnClickRegistration { get; private set; }
        public ICommand OnClickBack { get; private set; }
 
        #endregion

        public IWindowContainer Container { get; private set; }

        private string _name;
        private string _surname;
        private string _lastname;
        private string _login;
        private string _password;
        private string _confirm_password;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }
        public string Lastname
        {
            get => _lastname;
            set => this.RaiseAndSetIfChanged(ref _lastname, value);
        }
        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }
        public string ConfirmPassword
        {
            get => _confirm_password;
            set => this.RaiseAndSetIfChanged(ref _confirm_password, value);
        }


        public RegistrationViewModel(IWindowContainer container, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            Container = container;

            var checkPass = this
                .WhenAnyValue(
                    x => x.Password,
                    x => x.ConfirmPassword,
                    (pass, con_pass) => pass == con_pass);


            OnClickRegistration = ReactiveCommand.Create(() =>
            {
                RegistrationModel model = new RegistrationModel
                {
                    Login = Login,
                    Password = Password,
                    Name = Name
                };

                bool ok = SignalRConnection.sendRegistration(model);
                if (ok)
                {
                    if (container != null)
                    {
                        container.GoToHome();
                    }
                }
            }, checkPass);

            OnClickBack = ReactiveCommand.Create(() =>
            {
                if(container != null)
                {
                    container.GoToAuthorization();
                }
            });
        }

        public string? UrlPathSegment => "/registration";

        public IScreen HostScreen { get; }
    }
}
