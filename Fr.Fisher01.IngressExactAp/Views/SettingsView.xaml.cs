
using Acr.UserDialogs;
using Fr.Fisher01.IngressExactAp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fr.Fisher01.IngressExactAp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView : ContentPage
    {
        private SettingsViewModel _viewModel;

        public SettingsView()
        {
            InitializeComponent();

            BindingContext = _viewModel = new SettingsViewModel(UserDialogs.Instance);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.ViewDidAppear();
        }
    }
}