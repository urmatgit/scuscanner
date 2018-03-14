using ReactiveUI;
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
    public abstract class AbstractViewModel : ReactiveObject, IViewModel
    {
        public virtual void Init(object args)
        {
        }


        public virtual void OnActivate()
        {
        }


        public virtual void OnDeactivate()
        {
        }
    }
    public class BaseViewModel : AbstractViewModel
    {
        public Settings SettingsBase => Settings.Current;
         
          
        

        bool busy;
        public bool IsBusy
        {
            get => this.busy;
             set => this.RaiseAndSetIfChanged(ref this.busy, value);
        }

        protected LocalizedResources resources;
        public virtual LocalizedResources Resources
        {
            get => resources;
            private set => this.RaiseAndSetIfChanged(ref resources, value);
        }

        public BaseViewModel()
        {

            this.WhenAnyValue(vm => vm.SettingsBase.Resources).Subscribe(val =>
            {
                Resources = val;
            });

            SettingsBase.Resources = new LocalizedResources(typeof(AppResource), App.CurrentLanguage);//  App.CurrentLanguage);

        }
        public void SetResourcesLang(string lang)
        {
            SettingsBase.SetResourcesLang(lang);
            //Settings.Resources = new LocalizedResources(typeof(AppResource), lang);//  App.CurrentLanguage);
        }

        
    }
}
