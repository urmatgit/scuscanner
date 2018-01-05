using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SCUScanner.ViewModels
{
   public class SettingViewModel: INotifyPropertyChanged
    {
        
        public bool ContinuousScan
        {
            get
            {
                return Settings.ContinuousScan;

            }
            set
            {
                if (Settings.ContinuousScan == value) return;
                Settings.ContinuousScan = value;
                OnPropertyChanged();
                
            }
        }
        public bool ManualScan
        {
            get => Settings.ManualScan;
            set
            {
                if (Settings.ManualScan == value) return;
                Settings.ManualScan = value;
                OnPropertyChanged();

            }
        }
        public string SelectedLang
        {

            get => Settings.SelectedLanguageCode;
            set
            {
                if (Settings.SelectedLanguageCode == value) return;
                Settings.SelectedLanguageCode = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
