using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            get => isConnected;
            set=>  this.RaiseAndSetIfChanged(ref isConnected,value);
         }
        public int rssi;
        public int Rssi
        {
            get => rssi;
            set => this.RaiseAndSetIfChanged(ref rssi, value);
        }
        string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);

        }

        public DeviceListItemViewModel(IDevice device)
        {
            Device = device;
            Update(device);
        }
        public bool flgManualChangeName { get; set; } = false;
        public void UpdateButtonText()
        {
           // var devicename = Device?.Name;
            ConnectButtonText = IsConnected ? SettingsBase.Resources["DisConnectButtonText"] : SettingsBase.Resources["ConnectButtonText"];
        }
        string connectButtonText;
        public string ConnectButtonText
        {
            get => connectButtonText;
            set => this.RaiseAndSetIfChanged(ref this.connectButtonText, value);
        }

        public string Address
        {
            get
            {
                return Id.ToString() ;
                //if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                //    return Device?.ToString();
                //else
                //    return Device?.NativeDevice.ToString();
            }
        }
        public void Update(IDevice newDevice = null)
        {
            if (newDevice != null)
            {
                Device = newDevice;
            }
            IsConnected= Device.State == DeviceState.Connected;
            Rssi = Device.Rssi;
            Debug.WriteLine($"Update name {Device.Name} Old name {Name}");
            if (!flgManualChangeName)
            {
                Name = Device.Name;
            }
            UpdateButtonText();
            //RaisePropertyChanged(nameof(IsConnected));
            //RaisePropertyChanged(nameof(Rssi));
        }
    }
}
