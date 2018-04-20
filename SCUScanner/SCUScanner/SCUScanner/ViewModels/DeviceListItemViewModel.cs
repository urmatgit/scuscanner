using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.ViewModels
{
   public class DeviceListItemViewModel:BaseViewModel
    {
        public IDevice Device { get; private set; }

        public Guid Id => Device.Id;
        public bool isConnected;
        public bool IsConnected
        {
            get => IsConnected;
            set=>  this.RaiseAndSetIfChanged(ref isConnected,value);
         }
        public int rssi;
        public int Rssi
        {
            get => rssi;
            set => this.RaiseAndSetIfChanged(ref rssi, value);
        }
        public string Name => Device.Name;

        public DeviceListItemViewModel(IDevice device)
        {
            Device = device;
        }
        public void Update(IDevice newDevice = null)
        {
            if (newDevice != null)
            {
                Device = newDevice;
            }
            IsConnected= Device.State == DeviceState.Connected;
            Rssi = Device.Rssi; 
            //RaisePropertyChanged(nameof(IsConnected));
            //RaisePropertyChanged(nameof(Rssi));
        }
    }
}
