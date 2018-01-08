using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Plugin.BluetoothLE;
using SCUScanner.Services;

namespace SCUScanner.ViewModels
{
    public class ScanBluetoothViewModel:BaseViewModel
    {
       
        public ScanBluetoothViewModel()
        {
            var status = CrossBleAdapter.Current.Status;
            if (status == AdapterStatus.PoweredOn)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }

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
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
                OnPropertyChanged("IsVisibleBlueToothTornOff");
            }
        }
        public  LocalizedResources ResourcesEx
        {
            get
            {
                if (!isVisible) return null;
                return Resources;
            }
        }
    }
}
