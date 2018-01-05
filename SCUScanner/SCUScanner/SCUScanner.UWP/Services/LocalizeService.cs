using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: Xamarin.Forms.Dependency(typeof(SCUScanner.UWP.Services.LocalizeService))]
namespace SCUScanner.UWP.Services
{
  
    public class LocalizeService : SCUScanner.Services.ILocalizeService
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentCulture;
        }
    }
}
