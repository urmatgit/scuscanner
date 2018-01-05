using SCUScanner.Models;
using SCUScanner.Services;
using SCUScanner.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsLanguagePage : ContentPage
    {
        INavigation NavPage;
        LanguageItemViewModel viewModel;
        public SettingsLanguagePage(INavigation navPage )
        {
            NavPage = navPage;
            InitializeComponent();

            BindingContext = viewModel= new LanguageItemViewModel();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            LanguageItem selected = e.Item as LanguageItem;
            //await DisplayAlert("Item Tapped", $"An item was tapped.{selected.Name}", "OK");
           
            viewModel.Settings.Settings.SelectedLang = selected.Kod;
            //    viewModel.SetResourcesLang(viewModel.Settings.Settings.SelectedLang);
            //App.CurrentLanguage = SelectedLanguage;
            DependencyService.Get<ILocalizeService>().SetLocale(selected.Kod);
            MessagingCenter.Send<object, CultureChangedMessage>(viewModel.Settings.Settings,
                string.Empty, new CultureChangedMessage(viewModel.Settings.Settings.SelectedLang));
            
            await NavPage.PopAsync();
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
            
        }
    }
}
