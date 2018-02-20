using SCUScanner.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SCUItemsPage : ContentPage
    {
         

        public SCUItemsPage()
        {
            InitializeComponent();
            BindingContext = new SCUDatasViewModel();
           
        }

       
    }
}
