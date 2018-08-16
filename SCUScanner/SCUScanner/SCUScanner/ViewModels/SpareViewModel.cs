using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class SpareViewModel: BaseViewModel
    {
        private ImageSource imageSpare;
        public ImageSource ImageSpare
        {
            get =>   imageSpare; 
            set=> this.RaiseAndSetIfChanged(ref imageSpare, value);
        }
    }
}
