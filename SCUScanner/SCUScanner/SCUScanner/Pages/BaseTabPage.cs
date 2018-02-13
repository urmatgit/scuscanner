using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Pages
{
    public class BaseTabPage:ContentPage
    {
        public static readonly BindableProperty TabbedProperty =
                       BindableProperty.Create("Tabbed", typeof(TabbedPage), typeof(TabbedPage),
                 default(TabbedPage),defaultBindingMode: BindingMode.TwoWay);

        public TabbedPage Tabbed
        {
            get { return (TabbedPage)GetValue(TabbedProperty); }
            set {
                SetValue(TabbedProperty, value);
            }
        }
        public string Kod { get; set; }
    }
}
