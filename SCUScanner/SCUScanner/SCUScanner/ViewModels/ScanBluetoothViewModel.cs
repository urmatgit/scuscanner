using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Plugin.BluetoothLE;
using SCUScanner.Services;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class ScanBluetoothViewModel:BaseViewModel
    {

        IAdapter CurrentAdapter;
        public ScanBluetoothViewModel(Page page)
        {
            CurrentAdapter = CrossBleAdapter.Current;
            if(CurrentAdapter.Status == AdapterStatus.Unsupported)
            {
                IsVisible = false;
                 page.DisplayAlert("Блютуз не поддериживатся ", "", "OK");
                return;
            }
            if ( !CheckStatus(CurrentAdapter.Status) && CurrentAdapter.CanControlAdapterState())
            {
                CurrentAdapter.SetAdapterState(true);
                Debug.WriteLine("CurrentAdapter.SetAdapterState(true);");
            }
            
            CrossBleAdapter.Current.WhenStatusChanged().Subscribe(st=>
            {
                CheckStatus(st);
            });
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
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
                OnPropertyChanged("IsVisibleBlueToothTornOff");
                OnPropertyChanged("ResourcesEx");
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
