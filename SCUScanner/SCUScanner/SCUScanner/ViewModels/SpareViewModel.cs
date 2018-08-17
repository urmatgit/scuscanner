using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class SpareViewModel: BaseViewModel
    {
        public ICommand AddCommand { get; }
        private ImageSource imageSpare;
        public ImageSource ImageSpare
        {
            get =>   imageSpare; 
            set=> this.RaiseAndSetIfChanged(ref imageSpare, value);
        }
        public SpareViewModel()
        {
            //ImageSpare = ImageSource.FromFile(App.analizeSpare.LocalImagePath);
            AddCommand = ReactiveCommand.Create(() =>
            {
            var rnd = new Random();
            App.analizeSpare.Carts.AddCart(App.analizeSpare.CSVParser.Parts[rnd.Next(0, App.analizeSpare.CSVParser.Parts.Count - 1)]);
            });
        }

    }
}
