using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Timers;
using System.Windows.Input;
using System.ComponentModel;
namespace SCUScanner.ViewModels
{
    
    public class DevicesViewModel : BaseViewModel
    {

        public bool IsVisibleBlueToothTornOff
        {
            get
            {
                return !isVisibleLayout;
            }
        }

        /// <summary>
        /// Layout for scanning 
        /// </summary>
        private bool isVisibleLayout = false;
        public bool IsVisibleLayout
        {
            get => isVisibleLayout;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isVisibleLayout, value);


            }
        }
        public DevicesViewModel()
        {
            this.WhenAnyValue(vm => vm.IsVisibleLayout).ToProperty(this, x => x.IsVisibleBlueToothTornOff);
            IsVisibleLayout = App.BleAdapter.Status == AdapterStatus.PoweredOn;
            if (App.BleAdapter.Status == AdapterStatus.Unsupported || App.BleAdapter.Status == AdapterStatus.Unknown)
            {
                IsVisibleLayout = false;
                // return;
            }
            App.BleAdapter.WhenStatusChanged()
              .ObserveOn(RxApp.MainThreadScheduler)
              .Subscribe(st =>
              {

                  IsVisibleLayout = st == AdapterStatus.PoweredOn;
              });

        }

    }
}
