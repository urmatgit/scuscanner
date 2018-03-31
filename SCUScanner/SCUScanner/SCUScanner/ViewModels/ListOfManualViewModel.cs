using SCUScanner.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class ListOfManualViewModel:BaseViewModel
    {
       public string WorkManualDir { get; set; }
        public ObservableCollection<string> Items { get; set; }
        
        public ListOfManualViewModel()
        {
            
            WorkManualDir =  DependencyService.Get<ISQLite>().GetWorkManualDir();
            Items = new ObservableCollection<string>();
            AddManualsToItem();
        }
        private void AddManualsToItem()
        {
          
            foreach(string file in Directory.EnumerateFiles(WorkManualDir))
            {
                Items.Add(Path.GetFileNameWithoutExtension(file));
            }

        }
    }
}
