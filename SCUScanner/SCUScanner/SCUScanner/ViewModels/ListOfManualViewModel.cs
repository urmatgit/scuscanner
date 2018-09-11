using ReactiveUI;
using SCUScanner.Helpers;
using SCUScanner.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class mainualType
    {
        public string file { get; set; }
        
    }

    public class ListOfManualViewModel:BaseViewModel
    {
       public string WorkManualDir { get; set; }
        public ObservableCollection<mainualType> Items { get; set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LabelCommand { get; private set; }
        INavigation Navigation;
        public ListOfManualViewModel(INavigation navigation )
        {
            Navigation = navigation;
            WorkManualDir =  DependencyService.Get<ISQLite>().GetWorkManualDir();
            WorkManualDir = Path.Combine(WorkManualDir, "manuals");
            if (!Directory.Exists(WorkManualDir))
            {
                Directory.CreateDirectory(WorkManualDir);
            }
            Items = new ObservableCollection<mainualType>();
            DeleteCommand = ReactiveCommand.CreateFromTask<mainualType>(async (o) =>
             {
                 mainualType ofile = o as mainualType;
                 string file = Path.Combine(WorkManualDir, $"{ofile.file}.pdf");
                 if (File.Exists(file))
                 {
                     File.Delete(file);
                     Items.Remove(ofile);
                     App.Dialogs.Toast($"{Resources["DeletedText"]} {ofile.file}");
                 }
             });

            LabelCommand = ReactiveCommand.CreateFromTask<mainualType>(async (o) =>
            {
                mainualType ofile = o as mainualType;
                string file = Path.Combine(WorkManualDir, $"{ofile.file}.pdf");
                if (File.Exists(file))
                {
                 
                    WebViewPageCS webViewPageCS = new WebViewPageCS(file);
                    await Navigation.PushAsync(webViewPageCS);

                }
            });
            AddManualsToItem();
        }
        private void AddManualsToItem()
        {

            foreach (string file in Directory.EnumerateFiles(WorkManualDir))
            {

                Items.Add(new mainualType()
                {
                    file = Path.GetFileNameWithoutExtension(file )
                  
                });
            }

        }
    }
}
