using Acr.UserDialogs;
using Fr.Fisher01.IngressExactAp.ViewModels;
using Xamarin.Forms;

namespace Fr.Fisher01.IngressExactAp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton(new CalculatorViewModel(UserDialogs.Instance));
            DependencyService.RegisterSingleton(new SettingsViewModel(UserDialogs.Instance));

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
