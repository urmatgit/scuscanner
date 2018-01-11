using Plugin.Settings;
using Plugin.Settings.Abstractions;
using ReactiveUI;
using SCUScanner.Services;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Models
{
    public  class  Settings : BaseViewModel
    {

        private static ISettings AppSettings =>  CrossSettings.Current;
        static Settings settings;
        public static Settings Current =>
          settings ?? (settings = new Settings());
        public Settings()
        {
            this.WhenAnyValue(vm => vm.ScanMode).Subscribe(val => { ManualScan = !val; });
            this.WhenAnyValue(vm => vm.ManualScan).Subscribe(val => { ScanMode = !val; });
        }
        /// <summary>
        /// if false =Continuouse, true -manual
        /// </summary>
        bool scanmode;
        public  bool ScanMode
        {
            get => AppSettings.GetValueOrDefault(nameof(ScanMode), true);
             set
            {
                var original = ScanMode;
                if (AppSettings.AddOrUpdateValue(nameof(ScanMode), value))
                {
                    //SetProperty(ref original, value);
                    //OnPropertyChanged("ManualScan");
                    this.RaiseAndSetIfChanged(ref this.scanmode, value);
                 //   ManualScan = !this.scanmode;
                }
                
            }
        }
        bool manualscan;
        public bool ManualScan
        {
            get => manualscan;
            set
            {
                
                //ScanMode = !value;
                this.RaiseAndSetIfChanged(ref this.manualscan, value);
                //OnPropertyChanged();   
            }

        }
        string selectedlang;
        public  string SelectedLang
        {
            get => AppSettings.GetValueOrDefault(nameof(SelectedLang), "");
            set
            {
                var orinal = SelectedLang;
                
                if (AppSettings.AddOrUpdateValue(nameof(SelectedLang), value))
                {
                    App.CurrentLanguage = value;
                    SetResourcesLang(value);

                    this.RaiseAndSetIfChanged(ref this.selectedlang, value);

//                    SetProperty(ref orinal, value);
                }
            }
        }
    }
}

