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

