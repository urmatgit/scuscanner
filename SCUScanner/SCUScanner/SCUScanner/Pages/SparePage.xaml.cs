using Acr.UserDialogs;
using SCUScanner.ViewModels;
using Syncfusion.SfImageEditor.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SparePage :MR.Gestures.ContentPage
	{
        SpareViewModel spareViewModel;
		public SparePage ()
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.iOS)
            {

            }
            else
            {
                NavigationPage.SetHasNavigationBar(this, false);
                NavigationPage.SetHasBackButton(this, false);
            }
            spareViewModel = new SpareViewModel();
            spareViewModel.ImageSpare = ImageSource.FromFile(App.analizeSpare.LocalImagePath);
            BindingContext = spareViewModel;
           

            //editor.Source = imagesource;
             
            //var bytes= System.IO.File.ReadAllBytes(App.analizeSpare.LocalImagePath);
            //MemoryStream mem = new MemoryStream(bytes);

            //NavigationBarView.FirstNameLabel.Text = "3";

        }
        public static async Task InitSparePage()
        {
            App.analizeSpare = new Services.AnalizeSpare(App.SerialNumber);
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var config = new ProgressDialogConfig()
            {
                Title = $"{SCUScanner.Models.Settings.Current.Resources["DownloadWaitText"] }  ({App.SerialNumber})",
                //                CancelText = Models.Settings.Current.Resources["CancelText"],
                IsDeterministic = false,
                OnCancel = tokenSource.Cancel
            };
            //

            using (var progress = App.Dialogs.Progress(config))
            {
                progress.Show();
                await App.analizeSpare.ReadCSV(progress);
                //spareViewModel.ImageSpare = ImageSource.FromFile(App.analizeSpare.LocalImagePath);
            }
        }
        protected override void OnSizeAllocated(double width, double height)
        {
           

            base.OnSizeAllocated(width, height);
            //imgViewer.WidthRequest = width - NavigationBarView.Width;
        }
        protected override void OnAppearing()
        {
            
            spareViewModel.OnActivate();
            base.OnAppearing();
        }

        private async void ShowCartClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartsPage());
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.Dialogs.AlertAsync("Clicked");
        }
    }
}