using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Resources
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private readonly CultureInfo _cultureInfo;
       
        public TranslateExtension()
        {
           AppResource.Culture = DependencyService.Get<Services.ILocalizeService>().GetCurrentCultureInfo();
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return null;
            }
            
            var translation = AppResource.ResourceManager.GetString(Text, _cultureInfo);

#if DEBUG
            if (translation == null)
            {
                throw new ArgumentException(string.Format("Key {0} was not found for culture {1}.", Text, _cultureInfo));
            }
#endif
            return translation;
        }
    }
}
