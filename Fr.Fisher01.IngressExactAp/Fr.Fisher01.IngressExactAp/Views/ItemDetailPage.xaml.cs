using Fr.Fisher01.IngressExactAp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Fr.Fisher01.IngressExactAp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}