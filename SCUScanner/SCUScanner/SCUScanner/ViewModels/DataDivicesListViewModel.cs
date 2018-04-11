using ReactiveUI;
using SCUScanner.Models;
using SCUScanner.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{

    public class DataDivicesListViewModel : BaseViewModel
    {
        INavigation Navigation;
        public ICommand SelectCommand { get; }
        public ObservableCollection<DevicesItem> DevicesItems { get; private set; }
        public DataDivicesListViewModel(INavigation navigation)
        {
            Navigation = navigation;
            DevicesItems = new ObservableCollection<DevicesItem>();
            SelectCommand = ReactiveCommand.CreateFromTask<DevicesItem> (async (x) =>
          //  SelectCommand=ReactiveCommand.Create<DevicesItem>((x)=>
            {
                //Device.BeginInvokeOnMainThread(async () =>
                {
                    //await Navigation.PopAsync();
                   
                       await  Navigation.PushAsync(new SCUItemsListPage(x.UnitName,x.SerialNo));
                   
                }//);
                //await App.Dialogs.AlertAsync($"Selected click - {x.UnitName}" );
                
            });
        }
        public override void OnActivate()
        {
            base.OnActivate();
            FillDeviceList();
        }

        private async void FillDeviceList()
        {
            var list = await App.Database.GetDevicesItem();
            try
            {
                DevicesItems.Clear();
                list.ForEach(l => DevicesItems.Add(l));
                //SCUItems.  = new ObservableCollection<SCUItem>(list);
            }
            catch (Exception er)
            {
                await App.Dialogs.AlertAsync(er.Message);
            }
        }
    }
}
