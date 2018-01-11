using SCUScanner.Models;
using SCUScanner.Resources;
using SCUScanner.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        //public Settings Settings => Settings.Current;

        protected bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        private bool isBusy;
        public bool IsBusy
        {
            get =>  isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }
        protected LocalizedResources resources;
        public virtual LocalizedResources Resources
        {
            get
            {
                return resources;
            }
            private set
            {
                if (resources != value)
                {
                    resources = value;
                    OnPropertyChanged();
                }
            }
        }

        public BaseViewModel()
        {
            
            Resources = new LocalizedResources(typeof(AppResource), App.CurrentLanguage);//  App.CurrentLanguage);
        }
        public void SetResourcesLang(string lang)
        {
            Resources = new LocalizedResources(typeof(AppResource), lang);//  App.CurrentLanguage);
        }
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        #endregion
    }
}
