using AvaloniaCallCenter.Models;
using AvaloniaCallCenter.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaCallCenter.ViewModels
{
    [DataContract]
    public class AuthorizationWindowViewModel : ReactiveObject, IRoutableViewModel
    {
        public ICommand OnClickLogIn { get; private set; }
        public ICommand OnClickGoToRegistration { get; private set; }
        private string _password;
        private string _login;
        private string _title = "Authorization";

        public IWindowContainer Container { get; private set; }


        public AuthorizationWindowViewModel(IWindowContainer container, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            Container = container;
            
            var canLogin = this
                .WhenAnyValue(
                    x => x.Login,
                    x => x.Password,
                    (user, pass) => !string.IsNullOrWhiteSpace(user) &&
                                    !string.IsNullOrWhiteSpace(pass));

            // Привязанные к команде кнопки будут выключены, пока
            // пользовательский ввод не завершён.
            OnClickLogIn = ReactiveCommand.Create(async () =>
            {
                bool ok = await SignalRConnection.authServerAsync(Login, Password);

                Title = ok ? "Login secuess" : "Login failed";
                if (ok)
                {
                    if(Container != null)
                        Container.GoToHome();
                }
            }, canLogin);

            OnClickGoToRegistration = ReactiveCommand.Create(() =>
            {
                if (Container != null)
                    Container.GoToRegistartion();
            });
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "/login";


        [DataMember]
        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        // Пароль на диск не сохраняем из соображений безопасности!
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
    }
}
