using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(SCUScanner.iOS.Services.LocalizeService))]
namespace SCUScanner.iOS.Services
{
    public class LocalizeService : SCUScanner.Services.ILocalizeService
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var prefLanguage = "en-US";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = pref.Replace("_", "-"); // заменяет pt_BR на pt-BR
            }
            System.Globalization.CultureInfo ci = null;
            try
            {
                ci = new System.Globalization.CultureInfo(netLanguage);
            }
            catch
            {
                ci = new System.Globalization.CultureInfo(prefLanguage);
            }
            return ci;
        }

        public CultureInfo SetLocale(string ci)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(ci);// GetCultureInfo()
            if (culture != null)
            {
                Debug.WriteLine($"Set language- {culture.DisplayName}");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            return culture;
        }
    }
}