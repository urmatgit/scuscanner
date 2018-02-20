using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SCUScanner.ViewModels
{
    public class SCUDatasViewModel: AbstractViewModel
    {
        public ObservableCollection<SCUItem>  SCUItems { get; private set; }
        public SCUDatasViewModel()
        {
            SCUItems = new ObservableCollection<SCUItem>();
            //    FillItems();
            FillItems();
        }
        public override void OnActivate()
        {
            base.OnActivate();
            FillItems();

        }
        private async void FillItems()
        {
            var list = await App.Database.GetItemsAsync();
            try
            {
                list.ForEach(l => SCUItems.Add(l));
                //SCUItems.  = new ObservableCollection<SCUItem>(list);
            }catch(Exception er)
            {
                await App.Dialogs.AlertAsync(er.Message);
            }
            //SCUItems.Add(new SCUItem()
            //{
            //    ID = "SCU2-01",
            //    DateWithTime = DateTime.Now,
            //    Location= "DEV001",
            //    MacAddress= "28:47:76:10",
            //    Operator="User",
            //    Comment= "Text string (255 chars max?)",
            //    Speed=100
            //});
        }
    }
}
