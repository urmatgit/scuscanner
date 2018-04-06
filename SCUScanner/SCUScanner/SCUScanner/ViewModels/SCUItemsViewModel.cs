using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using System.Threading.Tasks;

namespace SCUScanner.ViewModels
{
    public class SCUItemsViewModel:BaseViewModel
    {
        private const int MaxPageItem = 5;
        private string UnitName { get; set; }
        public ICommand RefreshCommand { get; }
        public ObservableCollection<SCUItem> SCUItems { get; set; }
        private int ItemCounts { get; set; }
        private int LoadedItemCount;
        public SCUItemsViewModel(string unitName)
        {
            UnitName = unitName;
            SCUItems = new ObservableCollection<SCUItem>();
            RefreshCommand= ReactiveCommand.CreateFromTask(async () =>
            {
                await GetNextItems();
            });
            GetFisrtPage();
        
        }
        private async void GetFisrtPage()
        {
            ItemCounts = await App.Database.GetItemAsyncCount(UnitName.Trim());
            
            var items = await App.Database.GetItemAsync(UnitName, 0, MaxPageItem);
            items.ForEach(l => SCUItems.Add(l));
            LoadedItemCount = MaxPageItem;
        }
        public async Task GetNextItems()
        {
            if (LoadedItemCount > ItemCounts) return;
            
            var items = await App.Database.GetItemAsync(UnitName, LoadedItemCount, MaxPageItem);
            items.ForEach(l => SCUItems.Add(l));
            LoadedItemCount += MaxPageItem;
            
        }

    }
}
