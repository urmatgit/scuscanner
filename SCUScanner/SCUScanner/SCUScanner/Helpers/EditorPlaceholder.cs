using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    public class PlaceholderEditor : Editor
    {
        public static BindableProperty PlaceholderProperty
            = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PlaceholderEditor),string.Empty, propertyChanging: OnTextChanged, defaultBindingMode: BindingMode.OneWay);
        public PlaceholderEditor()
        {
            BindingContext = this;
        }
        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //throw new NotImplementedException();

        }

        public static BindableProperty PlaceholderColorProperty
            = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(PlaceholderEditor), Color.Gray);

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
    }
}
