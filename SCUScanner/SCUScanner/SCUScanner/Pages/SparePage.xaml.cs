using Acr.UserDialogs;
using SCUScanner.Helpers;
using SCUScanner.Models;
using SCUScanner.ViewModels;
using Syncfusion.SfImageEditor.XForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public partial class SparePage : ContentPage
	{
        const int OrgImageWidth = 4958;
        const int OrgImageHeight = 7015;

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
            
            BindingContext = spareViewModel;

            imgViewer.ImageLoaded += ImgViewer_ImageLoaded;
            NavigationBarView.OnOptionOK += NavigationBarView_OnOptionOK;
            //editor.Source = imagesource;

            //var bytes= System.IO.File.ReadAllBytes(App.analizeSpare.LocalImagePath);
            //MemoryStream mem = new MemoryStream(bytes);

            //NavigationBarView.FirstNameLabel.Text = "3";

        }

        private void NavigationBarView_OnOptionOK(object sender, EventArgsShowBorderChange e)
        {
            if(e.OldValue!=e.NewValue)
            {
                DrawPartBorder(e.NewValue);

            }
        }

        private void AddButtons()
        {
            double scaleX = imgViewer.Bounds.Width * 100 / OrgImageWidth;
            double scaleY = imgViewer.Bounds.Height * 100 / OrgImageHeight;
            foreach(Part part in App.analizeSpare.CSVParser.Parts)
            {
                PartButton button = new PartButton(part);
                button.OnLongPressed += Button_OnLongPressed;

            
                button.Text = part.PartNumber;
                absLayout.Children.Add(button, new Rectangle(part.Rect.X / scaleX, part.Rect.Y / scaleY, part.Rect.Width / scaleX, part.Rect.Height / scaleY));

            }
        }
        private void DrawPartBorder(bool draw)
        {
            foreach(PartButton button in  absLayout.Children.Where(c=>c is PartButton))
            {
                if (draw)
                    button.DrawBorder();
                else
                    button.RemoveBorder();
            }
        }
        private   void Button_OnLongPressed(object sender, EventArgs e)
        {
            PartButton button = sender as PartButton;
             App.Dialogs.ActionSheet (new ActionSheetConfig()
                .SetTitle($"{button.Part.PartNumber},{button.Part.PartName}")
                .Add(Models.Settings.Current.Resources["AddToOrderText"], () =>
                {
                    App.analizeSpare.vmCarts.AddCart(button.Part);
                    spareViewModel.CartCount = App.analizeSpare.vmCarts.TotalSum().ToString();
                })
                .SetCancel(Models.Settings.Current.Resources["CancelText"])
                );


               
        }

        private void ImgViewer_ImageLoaded(object sender, ImageLoadedEventArgs args)
        {
            
           
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
            AddButtons();
        }

        private async void ShowCartClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartsPage());
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"Page bound (w,h) {this.Bounds.Width} -{this.Bounds.Height} ");
            Debug.WriteLine($"Abslayout bound (w,h) {this.absLayout.Width} -{this.absLayout.Bounds.Height} ");
            Debug.WriteLine($"SFImageEdit bound (w,h) {this.imgViewer. Bounds.Width} -{this.imgViewer.Bounds.Height} ");
            Button btn = sender as Button;
            await App.Dialogs.AlertAsync(btn.Text);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Frame btn = sender as Frame;
            await App.Dialogs.AlertAsync(btn.Id.ToString());
        }

        

        
    }
}