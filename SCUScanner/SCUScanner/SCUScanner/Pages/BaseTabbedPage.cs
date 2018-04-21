using SCUScanner.Services;
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

           // (this.BindingContext as IViewModel)?.OnActivate();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //(this.BindingContext as IViewModel)?.OnDeactivate();
        }
    }
}
