using ReactiveUI;
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
	public partial class CharacteristicView : ContentView
    {
       public ConnectedDeviceViewModel ConnectedViewModel { get; set; }
        public CharacteristicView(ScanResultViewModel viewModel)
		{
            ConnectedViewModel = new ConnectedDeviceViewModel(viewModel);
            InitializeComponent ();
            
            BindingContext = this;

        }
        
        //protected override void OnAppearing()
        //{
        //    connectedDeviceViewModel.ParentTabbed = this.Tabbed;
        //    base.OnAppearing();
        //    (this.BindingContext as IViewModel)?.OnActivate();
        //}

        
        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    (this.BindingContext as IViewModel)?.OnDeactivate();
        //}
        //public override void Dispose()
        //{
        //    connectedDeviceViewModel.Dispose();
        //    base.Dispose();
        //}
    }
}