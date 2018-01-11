using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.BluetoothLE;
using ReactiveUI;
using SCUScanner.Services;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class ScanBluetoothViewModel:  BaseViewModel
    {

        
        public ObservableCollection<IAdapter> Adapters { get; } = new ObservableCollection<IAdapter>();
        public ICommand Select { get; }
        public ICommand Scan { get; }

        public ScanBluetoothViewModel(Page page)
        {
        
            IsVisible = CrossBleAdapter.AdapterScanner.IsSupported;
            this.WhenAnyValue(vm => vm.IsVisible).ToProperty(this, x => x.IsVisibleBlueToothTornOff);
            this.WhenAnyValue(vm => vm.IsVisible).Subscribe(s =>
            {
                ResourcesEx = Resources;
            });

            //    OnPropertyChanged("IsVisibleBlueToothTornOff");
            //    OnPropertyChanged("ResourcesEx");
            //});


            CrossBleAdapter.Current.WhenStatusChanged().Subscribe(st=>
            {
                CheckStatus(st);
                
            });
            this.Select =new Command( () =>
            {
               this.IsBusy = true;
            //    var ad = adapter;
                App.Dialogs.AlertAsync("Selected");
                //CrossBleAdapter.Current = adapter;
                IsBusy = false;
            });
            this.Scan = new Command(ScanCommand);
            
        }
        public async void ScanCommand()
        {
            
                  this.IsBusy = true;
                App.BleAdapterScanner
                    .FindAdapters()
                    //  .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        this.Adapters.Add,
                        async () =>
                        {
                                        this.IsBusy = false;
                            switch (this.Adapters.Count)
                            {
                                case 0:
                                    App.Dialogs.Alert("No BluetoothLE Adapters Found");
                                    break;

                                case 1:
                                    CrossBleAdapter.Current = this.Adapters.First();
                                    // await vmManager.Push<MainViewModel>();
                                    break;
                            }
                        }
                    );
            
        }
        private bool CheckStatus(AdapterStatus status)
        {
            if (status == AdapterStatus.PoweredOn)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
            return isVisible;
        }
        public string BlueToothTornOffText
        {
            get { return $"{Resources["BlueToothTornOffText"]} {Resources["ScanText"]}" ; }
        }
        
        public bool IsVisibleBlueToothTornOff
        {
            get {
                return !isVisible;
            }
        }
        private bool isVisible=false;
        public bool IsVisible
        {
            get  =>  isVisible; 
            set 
            {
                this.RaiseAndSetIfChanged(ref this.isVisible, value);
                
            
            }
        }
        LocalizedResources resourcesex;
        public  LocalizedResources ResourcesEx
        {
            get
            {
            
                return resourcesex;
            }
             set
            {
                if (!isVisible)
                    resourcesex = null;
                else
                    resourcesex = value;
                this.RaiseAndSetIfChanged(ref this.resourcesex, value);
            }
        }
    }
}

