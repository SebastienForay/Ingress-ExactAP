using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fr.Fisher01.IngressExactAp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumberPicker : ContentView
    {
        public static readonly BindableProperty CountProperty = BindableProperty.Create("Count", typeof(int), typeof(int), 0, BindingMode.TwoWay);

        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public NumberPicker()
        {
            InitializeComponent();
        }

        private void MinusLabel_OnTapped(object sender, EventArgs e)
        {
            if (Count > 0)
                Count--;
        }

        private void PlusLabel_OnTapped(object sender, EventArgs e)
        {
            Count++;
        }
    }
}