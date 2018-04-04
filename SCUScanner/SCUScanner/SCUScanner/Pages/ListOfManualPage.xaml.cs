using SCUScanner.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListOfManualPage : ContentPage
    {
        ListOfManualViewModel viewModel;

        public ListOfManualPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ListOfManualViewModel();


           
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            string filename = e.Item as string;
            filename = Path.Combine(viewModel.WorkManualDir, filename+".pdf");
            WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
            await Navigation.PushAsync(webViewPageCS);

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
