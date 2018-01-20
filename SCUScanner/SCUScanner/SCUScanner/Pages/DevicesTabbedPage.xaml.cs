using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicesTabbedPage : TabbedPage
    {
        public DevicesTabbedPage ()
        {
            InitializeComponent();
            BindingContext = new BaseViewModel();
        }
    }
}