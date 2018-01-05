using SCUScanner.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Helpers
{
   public static class TranslateExtension
    {
        public static string Translate(this ILocalizeService localizeService, string str)
        {
            var tranlation = SCUScanner.Resources.AppResource.ResourceManager.GetString(str, localizeService.GetCurrentCultureInfo());
            return string.IsNullOrEmpty(tranlation) ? str : tranlation;
        }
    }
}
