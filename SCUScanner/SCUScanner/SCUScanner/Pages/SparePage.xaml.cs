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
	public partial class SparePage : ContentPage
	{
        SpareViewModel spareViewModel;
		public SparePage ()
		{
			InitializeComponent ();
            BindingContext = spareViewModel = new SpareViewModel();
            spareViewModel.ImageSpare = ImageSource.FromFile(App.analizeSpare.LocalImagePath);
            
		}
        protected override void OnAppearing()
        {
            
            base.OnAppearing();
        }
    }
}