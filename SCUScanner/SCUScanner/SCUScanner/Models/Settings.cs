using Plugin.Settings;
using Plugin.Settings.Abstractions;
using ReactiveUI;
using SCUScanner.Resources;
using SCUScanner.Services;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Models
{
    public  class  Settings : AbstractViewModel
    {

        private static ISettings AppSettings =>  CrossSettings.Current;
        static Settings settings;
        public static Settings Current =>
          settings ?? (settings = new Settings());
        public Settings():base()
        {
            this.WhenAnyValue(vm => vm.AutoScan).Subscribe(val => { ManualScan = !val; });
            this.WhenAnyValue(vm => vm.ManualScan).Subscribe(val => { AutoScan = !val; });
        }
        bool showDebugJson = false;
        public bool ShowDebugJson
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowDebugJson), false);
            set
            {
                var original = ShowDebugJson;
                if (AppSettings.AddOrUpdateValue(nameof(ShowDebugJson), value))
                {
                    this.RaiseAndSetIfChanged(ref this.showDebugJson, value);
                }
            }
        }
        //
         bool showSerialNumber = false;
        public bool ShowSerialNumber
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowSerialNumber), false);
            set
            {
                var original = ShowDSerialNumber;
                if (AppSettings.AddOrUpdateValue(nameof(ShowSerialNumber), value))
                {
                    this.RaiseAndSetIfChanged(ref this.showSerialNumber, value);
                }
            }
        }
        
        bool showPartBorder = true;
        public bool ShowPartBoder
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowPartBoder), true);
            set
            {
                if (AppSettings.AddOrUpdateValue(nameof(ShowPartBoder), value))
                {
                    //SetProperty(ref original, value);
                    //OnPropertyChanged("ManualScan");
                    this.RaiseAndSetIfChanged(ref this.showPartBorder, value);
                    //   ManualScan = !this.scanmode;
                }
            }
        }
        bool showPartNumber = false;
        public bool ShowPartNumber
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowPartNumber), false);
            set
            {
                if (AppSettings.AddOrUpdateValue(nameof(ShowPartNumber), value))
                {
                    //SetProperty(ref original, value);
                    //OnPropertyChanged("ManualScan");
                    this.RaiseAndSetIfChanged(ref this.showPartNumber, value);
                    //   ManualScan = !this.scanmode;
                }
            }
        }
        /// <summary>
        /// if false =Continuouse, true -manual
        /// </summary>
        bool autoscan;
        public  bool AutoScan
        {
            get => AppSettings.GetValueOrDefault(nameof(AutoScan), true);
             set
            {
                var original = AutoScan;
                if (AppSettings.AddOrUpdateValue(nameof(AutoScan), value))
                {
                    //SetProperty(ref original, value);
                    //OnPropertyChanged("ManualScan");
                    this.RaiseAndSetIfChanged(ref this.autoscan, value);
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
        string operatorname;
        public string OperatorName
        {
            get => AppSettings.GetValueOrDefault(nameof(OperatorName), "");
            set
            {
                var original = OperatorName;
                if (AppSettings.AddOrUpdateValue(nameof(OperatorName),value))
                {
                    this.RaiseAndSetIfChanged(ref this.operatorname, value);
                }
            }
        }
        string devicefilter;
        public string DeviceFilter
        {
            get => AppSettings.GetValueOrDefault(nameof(DeviceFilter), "");
            set
            {
                var original = DeviceFilter;
                if (AppSettings.AddOrUpdateValue(nameof(DeviceFilter), value))
                {
                    this.RaiseAndSetIfChanged(ref this.devicefilter, value);
                }
            }
        }
        string selectedlangKod;
        public string SelectedLangKod
        {
            get
            {
                var kod= AppSettings.GetValueOrDefault(nameof(SelectedLangKod), "");
                if (string.IsNullOrEmpty(kod))
                {
                    kod = SelectedLang.Substring(0, 2);
                }
                return kod;
            }
            set
            {
                var orinal = SelectedLang;

                if (AppSettings.AddOrUpdateValue(nameof(SelectedLangKod), value))
                {
                    
                    this.RaiseAndSetIfChanged(ref this.selectedlangKod, value);
                    //                    SetProperty(ref orinal, value);
                }
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
        int _SelectedColumnIndex = -1;
        public int SelectedColumnIndex
        {
            get => AppSettings.GetValueOrDefault(nameof(SelectedColumnIndex),0);
            set
            {
                var original = SelectedColumnIndex;
                if (AppSettings.AddOrUpdateValue(nameof(SelectedColumnIndex), value))
                {
                    this.RaiseAndSetIfChanged(ref _SelectedColumnIndex, value);
                }
            }
        }
        int _SelectedConditionIndex = -1;
        public int SelectedConditionIndex
        {
            get => AppSettings.GetValueOrDefault(nameof(SelectedConditionIndex),0);
            set
            {
                if (AppSettings.AddOrUpdateValue(nameof(SelectedConditionIndex), value))
                {
                    this.RaiseAndSetIfChanged(ref _SelectedConditionIndex, value);
                }
            }
        }
        private LocalizedResources resources;
        public  LocalizedResources Resources
        {
            get
            {
                return resources;
            }
             set => this.RaiseAndSetIfChanged(ref this.resources, value);

        }
        public void SetResourcesLang(string lang)
        {
            Resources = new LocalizedResources(typeof(AppResource), lang);//  App.CurrentLanguage);
        }
    }
}

