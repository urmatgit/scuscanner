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
    public  class BaseViewModel : ReactiveObject, IViewModel
    {
        public Settings Settings => Settings.Current;


        bool busy;
        public bool IsBusy
        {
            get => this.busy;
             set => this.RaiseAndSetIfChanged(ref this.busy, value);
        }
        protected LocalizedResources resources;
        public virtual LocalizedResources Resources
        {
            get
            {
                return resources;
            }
            private set => this.RaiseAndSetIfChanged(ref this.resources, value);
            
        }

        public BaseViewModel()
        {
            
            Resources = new LocalizedResources(typeof(AppResource), App.CurrentLanguage);//  App.CurrentLanguage);
        }
        public void SetResourcesLang(string lang)
        {
            Resources = new LocalizedResources(typeof(AppResource), lang);//  App.CurrentLanguage);
        }

        public virtual void Init(object args = null)
        {
            throw new NotImplementedException();
        }

        public virtual void OnActivate()
        {
            throw new NotImplementedException();
        }

        public virtual void OnDeactivate()
        {
            throw new NotImplementedException();
        }
    }
}
