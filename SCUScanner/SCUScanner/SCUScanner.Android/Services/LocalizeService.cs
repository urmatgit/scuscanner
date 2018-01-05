using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(SCUScanner.Droid.Services.LocalizeService))]
namespace SCUScanner.Droid.Services
{
    public class LocalizeService : SCUScanner.Services.ILocalizeService
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-");
            return new System.Globalization.CultureInfo(netLanguage);
        }

        public CultureInfo SetLocale(string ci)
        {
            var cultureINfor = CultureInfo.GetCultureInfo(ci);
            if (cultureINfor != null)
            {
                Thread.CurrentThread.CurrentCulture = cultureINfor;
                Thread.CurrentThread.CurrentUICulture = cultureINfor;
            }
            return cultureINfor;
        }
    }
}