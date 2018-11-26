using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Settings : ContentPage
	{
		public Settings ()
		{
            var vm= new SettingViewModel(Navigation);
            InitializeComponent();
            //Application.Current.MainPage as MasterDetailPage
            BindingContext = vm;
            swShowDebug.IsVisible = vm.FactoryMode;
            swShowSerial.IsVisible = vm.FactoryMode;
        }
	}
}