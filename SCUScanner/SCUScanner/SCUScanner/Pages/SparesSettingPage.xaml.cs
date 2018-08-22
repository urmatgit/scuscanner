using SCUScanner.Helpers;
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
	public partial class SparesSettingPage : ContentPage
	{
        public event EventHandler<EventArgsShowBorderChange> OnOptionOK;
        EventArgsShowBorderChange eventArg;
        public SparesSettingPage (EventHandler<EventArgsShowBorderChange> onOptionOK)
		{
			InitializeComponent ();
            BindingContext = new BaseViewModel();
            OnOptionOK = onOptionOK;
            eventArg = new EventArgsShowBorderChange();
            eventArg.OldValue = Models.Settings.Current.ShowPartBoder;
        }

        private async void OnDismissButtonClicked(object sender, EventArgs e)
        {
            eventArg.NewValue= Models.Settings.Current.ShowPartBoder;
            OnOptionOK?.Invoke(sender, eventArg);
            await Navigation.PopModalAsync();
        }
    }
}