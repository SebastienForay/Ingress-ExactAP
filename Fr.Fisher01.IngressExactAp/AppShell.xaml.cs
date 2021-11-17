using Xamarin.Forms;

namespace Fr.Fisher01.IngressExactAp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        
            SetNavBarIsVisible(this, false);
        }

    }
}
