using Plugin.BluetoothLE;
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
    }
}
