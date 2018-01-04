using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace SCUScanner.Services
{
    public interface ILocalizeService
    {
        CultureInfo GetCurrentCultureInfo();
    }
}
