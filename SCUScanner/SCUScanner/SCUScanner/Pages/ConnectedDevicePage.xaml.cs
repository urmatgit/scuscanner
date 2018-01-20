using SCUScanner.Services;
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
	public partial class ConnectedDevicePage : ContentPage
	{
		public ConnectedDevicePage (ScanResultViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext =new ConnectedDeviceViewModel(viewModel);

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (this.BindingContext as IViewModel)?.OnActivate();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (this.BindingContext as IViewModel)?.OnDeactivate();
        }
    }
}