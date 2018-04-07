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
using Plugin.Share;
using Plugin.Share.Abstractions;
using System.Threading;

namespace SCUScanner.ViewModels
{
    public class SCUItemsViewModel:BaseViewModel
    {
        private const int MaxPageItem = 5;
        private string UnitName { get; set; }
        public ICommand LoadMoreCommand { get; }
        public ICommand ShareCommand { get; }
        public ICommand DeleteCommand { get;  }
        public ObservableCollection<SCUItem> SCUItems { get; set; }
        private int ItemCounts { get; set; }




        public int LoadedItemCount;
        
        public SCUItemsViewModel(string unitName)
        {
            IsBusy = false;
            UnitName = unitName;
            SCUItems = new ObservableCollection<SCUItem>();
            
            LoadMoreCommand = new Command<object>(GetNextItems, CanLoadMoreItems);
            ShareCommand = ReactiveCommand.CreateFromTask(async () =>
            {

                if (!CrossShare.IsSupported)
                    return;
                var selecteItems = SCUItems.Where(s => s.IsSelected);
                StringBuilder stringBuilder = new StringBuilder();
                int CaptionLenth = 25;
                foreach (var str in selecteItems)
                {

                    string line = Resources["UnitNameText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.UnitName}");

                    line = Resources["SerialNoText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.SerialNo}");

                    line = Resources["RMPText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.Speed} {Resources["AlarmText"]} {str.AlarmSpeed}");

                    line = Resources["HoursRunText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.HoursElapsed} {Resources["AlarmText"]} {str.AlarmHours}");

                    line = Resources["LocationNameText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.Location}");

                    line = Resources["OperatorText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.Operator}");

                    line = Resources["NotesText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.Notes}");

                    line = Resources["DateWithTimeText"];
                    stringBuilder.AppendLine($"{line.PadRight(CaptionLenth, ' ')}: {str.DateWithTime}");
                    stringBuilder.AppendLine($"{"".PadRight(CaptionLenth*2, '-')}");
                }

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = "Reception text",
                    Text = stringBuilder.ToString()

                });

            });
            DeleteCommand= ReactiveCommand.CreateFromTask(async () =>
            {

                var selecteItems = SCUItems.Where(s => s.IsSelected);
                StringBuilder stringBuilder = new StringBuilder();
                try
                {
                    using (var cancelSrc = new CancellationTokenSource())
                    {
                        using (App.Dialogs.Loading(Resources["DeletingText"], cancelSrc.Cancel, Resources["CancelText"]))
                        {
                            foreach (var str in selecteItems)
                            {
                                
                                await App.Database.DeleteItemAsync(str);

                            }
                            //stringBuilder.AppendLine(str.UnitName);
                        }
                    }
                }
                finally
                {

                    await GetFisrtPage();
                }
                //await App.Dialogs.AlertAsync(stringBuilder.ToString());



            });
            Task.Run(()=>  GetFisrtPage());
        //    CanLoadMoreItems = this.WhenAnyValue(vm => LoadedItemCount > ItemCounts);
        }
        private bool CanLoadMoreItems(object obj)
        {
            if (LoadedItemCount > ItemCounts) return false;
            
            return true;
        }
        private async    Task GetFisrtPage()
        {
            try
            {
                IsBusy = true;
                ItemCounts = await App.Database.GetItemAsyncCount(UnitName.Trim());
                SCUItems.Clear();
                var items = await App.Database.GetItemAsync(UnitName, 0, MaxPageItem);
                foreach(var item in  items.OrderByDescending(i=>i.DateWithTime))
                    SCUItems.Add(item);
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
            await Task.Delay(1000);
            try
            {
                var items = await App.Database.GetItemAsync(UnitName, LoadedItemCount, MaxPageItem);
                foreach(var item in items.OrderByDescending(i=>i.DateWithTime))
                    SCUItems.Add(item);
                LoadedItemCount += MaxPageItem;
            }
            finally
            {
                IsBusy = false;
            }
            
        }

    }
}
