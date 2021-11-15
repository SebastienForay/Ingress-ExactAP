
using Fr.Fisher01.IngressExactAp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fr.Fisher01.IngressExactAp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculatorView : ContentPage
    {
        public CalculatorView()
        {
            InitializeComponent();

            BindingContext = DependencyService.Get<CalculatorViewModel>();
        }
    }
}