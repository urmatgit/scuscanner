using SCUScanner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigationBarView : ContentView
    {
        public event EventHandler<EventArgsShowBorderChange> OnOptionOK;
		public NavigationBarView ()
		{
			InitializeComponent ();
		}
        public Label FirstNameLabel
        {
            get
            {
                return labelText;
            }
        }
        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
        "Title",
        typeof(string),
        typeof(NavigationBarView),
        "this is Title",
        propertyChanged: OnTitlePropertyChanged
        );
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        static void OnTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisView = bindable as NavigationBarView;
            var title = newValue.ToString();
            thisView.labelText.Text = title;
        }
        private async void Tapcart_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CartsPage());
        }

        private async void tapOption_Tapped(object sender, EventArgs e)
        {
        
            await Navigation.PushModalAsync(new SparesSettingPage(OnOptionOK));
        
            
        }
    }
}
