using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaCallCenter.ViewModels;
using AvaloniaCallCenter.Views;
using ReactiveUI;
using Splat;

namespace AvaloniaCallCenter
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // ������������ ������ �������������.
            Locator.CurrentMutable.RegisterConstant<IScreen>(new MainWindowViewModel());
            Locator.CurrentMutable.Register<IViewFor<HomeViewModel>>(() => new HomeView());
            Locator.CurrentMutable.Register<IViewFor<AuthorizationWindowViewModel>>(() => new AuthorizationWindowView());
            Locator.CurrentMutable.Register<IViewFor<RegistrationViewModel>>(() => new RegistrationView());

            // �������� �������� ������ ������������� � �������������� �������� ������.
            new MainWindow { DataContext = Locator.Current.GetService<IScreen>() }.Show();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
