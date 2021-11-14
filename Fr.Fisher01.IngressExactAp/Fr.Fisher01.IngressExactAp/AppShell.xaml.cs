using Fr.Fisher01.IngressExactAp.ViewModels;
using Fr.Fisher01.IngressExactAp.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Fr.Fisher01.IngressExactAp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
