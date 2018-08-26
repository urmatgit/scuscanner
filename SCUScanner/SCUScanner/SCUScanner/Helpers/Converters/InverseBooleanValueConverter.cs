
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers.Converters
{
    public class InverseBooleanValueConverter :  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;

            return !(val.HasValue ? val.Value : true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;

            return !(val.HasValue ? val.Value : true);
        }
    }
}
