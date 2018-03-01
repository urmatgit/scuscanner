using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SCUScanner.Models;
using SCUScanner.Services;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMasterDetailPage : MasterDetailPage
    {
        MasterDetailPageMenuItem selectedPage = null;
        Page CurrentPage = null;
        public MainMasterDetailPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {

                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                    BindingContext = null;
                    SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;
                    CurrentPage.Title = settings.Resources[selectedPage.PageCode];
                }
            });

            ListView_ItemSelected(null, new SelectedItemChangedEventArgs(MasterPage.viewModel.MenuItems.FirstOrDefault(m => m.Id == 0)));

        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterDetailPageMenuItem;
            if (item == null)
                return;
            selectedPage = item;
            var navigation = ((Application.Current.MainPage as Xamarin.Forms.MasterDetailPage)?.Detail as Xamarin.Forms.NavigationPage);
            var CurrentDetailPage=navigation?.CurrentPage ;
            if (CurrentDetailPage == null || CurrentDetailPage.GetType().Name!=item.TargetType.Name)
            {
                if (item.TargetType.Name==typeof(MainTabbedPage).Name)
                    if(App.mainTabbed ==null)
                    {
                        CurrentPage = (Page)Activator.CreateInstance(item.TargetType);
                        CurrentPage.Title = item.Title;
                        App.mainTabbed = CurrentPage as MainTabbedPage;
                    }
                    else
                    {
                        CurrentPage = App.mainTabbed;
                    }
                else 
                
                {
                    CurrentPage = (Page)Activator.CreateInstance(item.TargetType);
                    CurrentPage.Title = item.Title;
                }
                Detail = new NavigationPage(CurrentPage);
            }            
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}