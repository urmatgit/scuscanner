using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public ObservableCollection<MasterDetailPageMenuItem> MenuItems { get; set; }
        public string IconSource { get; set; }
        public MenuPageViewModel():base()
        {
            //Icon-Small.png for ios
            if (Device.Android == Device.RuntimePlatform)
                IconSource = "icon.png";
            else
                IconSource = "Icon-Small.png";
        }

        //#region INotifyPropertyChanged Implementation
        //public event PropertyChangedEventHandler PropertyChanged;
        //void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    if (PropertyChanged == null)
        //        return;

        //    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        //#endregion
    }
}
