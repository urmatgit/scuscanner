using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Pages
{
    public class BaseTabbedPage : TabbedPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = BindingContext as MvmBaseViewModel;
            viewModel?.Resume();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var viewModel = BindingContext as MvmBaseViewModel;
            viewModel?.Suspend();
        }
    }
}
