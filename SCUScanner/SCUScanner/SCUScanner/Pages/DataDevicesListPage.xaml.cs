using SCUScanner.Models;
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
           
            filterText.TextChanged += FilterText_TextChanged;
            filterText.Text = dataDivicesListViewModel.SettingsBase.DeviceFilter;
        }

        

        protected override void OnDisappearing()
        {
            dataDivicesListViewModel.SettingsBase.DeviceFilter = filterText.Text;
            base.OnDisappearing();
        }
        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (DataDevicesListView.DataSource != null)
            {
                this.DataDevicesListView.DataSource.Filter = FilterDevices;
                this.DataDevicesListView.DataSource.RefreshFilter();
            }
        }
        private bool FilterDevices(object obj)
        {
            if (filterText == null || filterText.Text == null)
                return true;

            var contacts = obj as DevicesItem;
            if (contacts.UnitName.ToLower().Contains(filterText.Text.ToLower())
                 || contacts.SerialNo.ToLower().Contains(filterText.Text.ToLower()))
                return true;
            else
                return false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            dataDivicesListViewModel.OnActivate();
        }

       

        private void selectcommand(object sender, EventArgs e)
        {

            var item=((TappedEventArgs)e).Parameter as DevicesItem;
            if (item!=null)
                dataDivicesListViewModel.SelectCommand.Execute(item);
        }

        

        private async void imageClick(object sender, EventArgs e)
        {
            var item = ((TappedEventArgs)e).Parameter as DevicesItem;
            if (item != null)
                await Navigation.PushAsync(new MaintenancePage(item.SerialNo));
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
