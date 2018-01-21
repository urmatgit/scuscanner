using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.ViewModels
{
   public class GattCharacteristicViewModel : BaseViewModel
    {
        IGattCharacteristic Characteristic { get; }
        public GattCharacteristicViewModel(IGattCharacteristic characteristic)
        {
            Characteristic = characteristic;
        }
        public Guid Uuid => this.Characteristic.Uuid;
        public Guid ServiceUuid => this.Characteristic.Service.Uuid;
        public string Description => this.Characteristic.Description;
        public string Properties => this.Characteristic.Properties.ToString();
        public bool CanNotify => this.Characteristic.CanNotify();
        string value;
        public string Value
        {
            get => this.value;
            private set => this.RaiseAndSetIfChanged(ref this.value, value);
        }


        bool notifying;
        public bool IsNotifying
        {
            get => this.notifying;
            private set => this.RaiseAndSetIfChanged(ref this.notifying, value);
        }


        bool valueAvailable;
        public bool IsValueAvailable
        {
            get => this.valueAvailable;
            private set => this.RaiseAndSetIfChanged(ref this.valueAvailable, value);
        }


        DateTime lastValue;
        public DateTime LastValue
        {
            get => this.lastValue;
            private set => this.RaiseAndSetIfChanged(ref this.lastValue, value);
        }
    }
}
