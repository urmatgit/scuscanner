using SCUScanner.Models;
using SCUScanner.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{

    public class LanguageItemViewModel : BaseViewModel
    {
        public ObservableCollection<LanguageItem> LanguageItems { get; set; }
        public LanguageItemViewModel()
        {
            LanguageItems = new ObservableCollection<LanguageItem>(

                 new[]
                        {
                            new LanguageItem(){Kod="en",Name="English" },
                            new LanguageItem(){Kod="ru",Name="Russian" },
                            new LanguageItem(){Kod="zh",Name="Chinese" },
                            new LanguageItem(){Kod="fr",Name="France" },
                            new LanguageItem(){Kod="de",Name="German" },
                            new LanguageItem(){Kod="it",Name="Italian" },
                            new LanguageItem(){Kod="pt",Name="Portuguese" },
                            new LanguageItem(){Kod="es",Name="Spanish" },
                        }
                 );
            
        }

       
       
    }
}
