﻿using Fr.Fisher01.IngressExactAp.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fr.Fisher01.IngressExactAp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculatorView : ContentPage
    {
        private CalculatorViewModel _viewModel;

        public CalculatorView()
        {
            InitializeComponent();

            BindingContext = _viewModel = DependencyService.Get<CalculatorViewModel>();
        }

        private void DoubleApLabel_TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            _viewModel.IsDoubleApEnabled = !_viewModel.IsDoubleApEnabled;
        }

    }
}