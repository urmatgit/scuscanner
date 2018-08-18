using Acr.UserDialogs;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SparePage : ContentPage
	{
        SpareViewModel spareViewModel;
		public SparePage ()
		{
			InitializeComponent ();
            BindingContext = spareViewModel = new SpareViewModel();
            spareViewModel.ImageSpare = ImageSource.FromFile(App.analizeSpare.LocalImagePath);
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
        protected override void OnAppearing()
        {
            spareViewModel.OnActivate();
            base.OnAppearing();
        }

        private async void ShowCartClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartsPage());
        }
    }
}