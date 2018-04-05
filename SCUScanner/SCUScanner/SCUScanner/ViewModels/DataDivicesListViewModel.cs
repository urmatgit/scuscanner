using ReactiveUI;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace SCUScanner.ViewModels
{

    public class DataDivicesListViewModel : BaseViewModel
    {
        public ICommand SelectCommand { get; }
        public ObservableCollection<DevicesItem> DevicesItems { get; private set; }
        public DataDivicesListViewModel()
        {
            DevicesItems = new ObservableCollection<DevicesItem>();
            SelectCommand = ReactiveCommand.CreateFromTask<DevicesItem> (async (x) =>
            {
                await App.Dialogs.AlertAsync($"Selected click - {x.UnitName}" );
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
