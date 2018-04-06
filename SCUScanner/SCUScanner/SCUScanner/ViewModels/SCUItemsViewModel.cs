using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
namespace SCUScanner.ViewModels
{
    public class SCUItemsViewModel:BaseViewModel
    {
        private const int MaxPageItem = 5;
        private string UnitName { get; set; }
        public ObservableCollection<SCUItem> SCUItems { get; set; }
        private int ItemCounts { get; set; }
        private int LoadedItemCount;
        public SCUItemsViewModel(string unitName)
        {
            UnitName = unitName;
            SCUItems = new ObservableCollection<SCUItem>();
            
            GetFisrtPage();
        
        }
        private async void GetFisrtPage()
        {
            ItemCounts = await App.Database.GetItemAsyncCount(UnitName);
            
            var items = await App.Database.GetItemAsync(UnitName, 0, MaxPageItem);
            items.ForEach(l => SCUItems.Add(l));
            LoadedItemCount = MaxPageItem;
        }
        private async void GetNextItems()
        {
            if (LoadedItemCount > ItemCounts) return;
            
            var items = await App.Database.GetItemAsync(UnitName, LoadedItemCount, MaxPageItem);
            items.ForEach(l => SCUItems.Add(l));
            LoadedItemCount += MaxPageItem;
            
        }

    }
}
