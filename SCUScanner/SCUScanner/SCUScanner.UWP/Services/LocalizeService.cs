using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;

[assembly: Xamarin.Forms.Dependency(typeof(SCUScanner.UWP.Services.LocalizeService))]
namespace SCUScanner.UWP.Services
{
  
    public class LocalizeService : SCUScanner.Services.ILocalizeService
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentCulture;
        }

        public CultureInfo SetLocale(string ci)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(ci);// GetCultureInfo()
            if (culture != null) {
                Debug.WriteLine($"Set language- {culture.DisplayName}");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            }
            return culture;
        }
    }
}
