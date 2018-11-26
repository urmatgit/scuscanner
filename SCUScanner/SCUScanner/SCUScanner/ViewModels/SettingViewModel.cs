using SCUScanner.Helpers;
using SCUScanner.Models;
using SCUScanner.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

//using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class SettingViewModel: BaseViewModel
    {

        INavigation Navigation;
        public ICommand SelectLangCommnad
        {
            protected set;
            get;
        }

            public SettingViewModel(INavigation NavPage)
        {
            Navigation = NavPage;
            SelectLangCommnad = new Command(OpenSettingsLanguagePage);
                                //new Command(OpenSettingsLanguagePage);
            Debug.WriteLine("SelectLangCommnad created");
        }
        public bool FactoryMode
        {
            get
            {
                return GlobalConstants.FactoryMode;
            }
        }
        //public  bool ScanMode
        //{
        //    get => Models.Settings.Current.ScanMode;
        //    set => Models.Settings.Current.ScanMode = value;
            
        //}
        //public bool ManualScan
        //{
        //    get => Models.Settings.Current.ManualScan;
        //    set => Models.Settings.Current.ManualScan = value;
        //}
        /// <summary>
        /// Open Language select page
        /// </summary>
        public  async void OpenSettingsLanguagePage()
        {
            Debug.WriteLine("OpenSettingsLanguagePage");
            await Navigation.PushAsync(new  SettingsLanguagePage(Navigation));
            //Application.Current.MainPage.Navigation.PushAsync(new NavigationPage( new SettingsLanguagePage()));
         
        }
      
    }
}
