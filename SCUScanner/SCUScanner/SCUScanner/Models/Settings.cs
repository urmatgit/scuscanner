using Plugin.Settings;
using Plugin.Settings.Abstractions;
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
        /// <summary>
        /// if false =Continuouse, true -manual
        /// </summary>
        public  bool ScanMode
        {
            get => AppSettings.GetValueOrDefault(nameof(ScanMode), true);
            set
            {
                var original = ScanMode;
                if (AppSettings.AddOrUpdateValue(nameof(ScanMode), value))
                    SetProperty(ref original, value);
            }
        }
        //public  bool ManualScan
        //{
        //    get => AppSettings.GetValueOrDefault(nameof(ManualScan), false);
        //    set {
        //        var orinal = ManualScan;
        //        if (AppSettings.AddOrUpdateValue(nameof(ManualScan), value))
        //            SetProperty(ref orinal, value);
        //            }

        //}
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
                

                    SetProperty(ref orinal, value);
                }
            }
        }
    }
}

