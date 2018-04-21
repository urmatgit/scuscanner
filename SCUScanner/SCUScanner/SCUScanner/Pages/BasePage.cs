using SCUScanner.Services;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Pages
{
    public class BasePage : ContentPage
    {
        public static readonly BindableProperty KodProperty =
                      BindableProperty.Create("Kod", typeof(string), typeof(string),
                default(string), defaultBindingMode: BindingMode.TwoWay);

        public string Kod
        {
            get { return (string)GetValue(KodProperty); }
            set
            {
                SetValue(KodProperty, value);
            }
        }
       
        protected override void OnAppearing()
        {
            base.OnAppearing();

            
            (this.BindingContext as IViewModel)?.OnActivate(Kod);
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (this.BindingContext as IViewModel)?.OnDeactivate(Kod);
        }
    }
}
