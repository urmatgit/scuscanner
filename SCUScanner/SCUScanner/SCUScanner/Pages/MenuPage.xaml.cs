using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SCUScanner.Models;
using SCUScanner.ViewModels;
using SCUScanner.Services;
using System.Resources;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;
        MenuPageViewModel viewModel;
        public MenuPage()
        {
            InitializeComponent();

             viewModel = new MenuPageViewModel();

            viewModel.MenuItems = CreateMenuItems();
                //new ObservableCollection<MasterDetailPageMenuItem>(
                //    new[]
                //        {
                //            new MasterDetailPageMenuItem(typeof(Settings)) { Id = 0, Title =SCUScanner.Resources.AppResource.SettingsText},
                //            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 1, Title = "Page 2" },
                //            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 2, Title = "Page 3" }
                //        });

            BindingContext = viewModel;
            ListView = MenuItemsListView;
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {
                
                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                    BindingContext = null;
                    SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;
                    viewModel.MenuItems = CreateMenuItems(settings);
                    BindingContext = viewModel;
                }
            });
        }
        
        private ObservableCollection<MasterDetailPageMenuItem> CreateMenuItems(Models.Settings settings =null)
        {
            
             return new ObservableCollection<MasterDetailPageMenuItem>(
                   new[]
                       {
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 0,PageCode="DataText",  Title =settings==null?SCUScanner.Resources.AppResource.DataText : settings.Resources["DataText"] },
                            new MasterDetailPageMenuItem(typeof(Settings)) { Id = 1, PageCode="SettingsText",Title =settings==null?SCUScanner.Resources.AppResource.SettingsText : settings.Resources["SettingsText"]},
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 1,PageCode="HelpText", Title = settings==null?SCUScanner.Resources.AppResource.HelpText : settings.Resources["HelpText"]  },
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 2,PageCode="MaintenanceText", Title = settings==null?SCUScanner.Resources.AppResource.MaintenanceText : settings.Resources["MaintenanceText"]  },
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 3,PageCode="SparesText", Title = settings==null?SCUScanner.Resources.AppResource.SparesText : settings.Resources["SparesText"]  }
                       });
        }

    }
}