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
    public partial class DataDevicesListPage : ContentPage
    {
     
        DataDivicesListViewModel dataDivicesListViewModel;
        public DataDevicesListPage()
        {
            InitializeComponent();
            BindingContext= dataDivicesListViewModel = new DataDivicesListViewModel(Navigation);
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            dataDivicesListViewModel.OnActivate();
        }
        //async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    if (e.Item == null)
        //        return;

        //    await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

        //    //Deselect Item
        //    ((ListView)sender).SelectedItem = null;
        //}
    }
}
