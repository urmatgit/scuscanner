using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class SCUItemsViewModel:BaseViewModel
    {
        private const int MaxPageItem = 5;
        private string UnitName { get; set; }
        public ICommand LoadMoreCommand { get; }
        public ObservableCollection<SCUItem> SCUItems { get; set; }
        private int ItemCounts { get; set; }




        public int LoadedItemCount;
        
        public SCUItemsViewModel(string unitName)
        {
            IsBusy = false;
            UnitName = unitName;
            SCUItems = new ObservableCollection<SCUItem>();
            
            LoadMoreCommand = new Command<object>(GetNextItems, CanLoadMoreItems);
            

            GetFisrtPage();
        //    CanLoadMoreItems = this.WhenAnyValue(vm => LoadedItemCount > ItemCounts);
        }
        private bool CanLoadMoreItems(object obj)
        {
            if (LoadedItemCount > ItemCounts) return false;
            
            return true;
        }
        private async void GetFisrtPage()
        {
            try
            {
                IsBusy = true;
                ItemCounts = await App.Database.GetItemAsyncCount(UnitName.Trim());

                var items = await App.Database.GetItemAsync(UnitName, 0, MaxPageItem);
                items.ForEach(l => SCUItems.Add(l));
                LoadedItemCount = MaxPageItem;
            }catch(Exception er)
            {
                await App.Dialogs.AlertAsync(er.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async void GetNextItems(object obj)
        {
           
            if (IsBusy || LoadedItemCount > ItemCounts) return;
            IsBusy = true;
            await Task.Delay(2500);
            try
            {
                var items = await App.Database.GetItemAsync(UnitName, LoadedItemCount, MaxPageItem);
                items.ForEach(l => SCUItems.Add(l));
                LoadedItemCount += MaxPageItem;
            }
            finally
            {
                IsBusy = false;
            }
            
        }

    }
}
