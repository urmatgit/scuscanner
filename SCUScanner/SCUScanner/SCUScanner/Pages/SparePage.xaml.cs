using Acr.UserDialogs;
using Plugin.DeviceOrientation;
using SCUScanner.Helpers;
using SCUScanner.Models;
using SCUScanner.ViewModels;

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
using SkiaSharp;
using SkiaSharp.Views;
namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SparePage : ContentPage
	{

        SKCanvas _SKCanvas;
        SpareViewModel spareViewModel;
        bool IsPartAdded = false;
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

             
            NavigationBarView.OnOptionOK += NavigationBarView_OnOptionOK;
            //editor.Source = imagesource;

            //var bytes= System.IO.File.ReadAllBytes(App.analizeSpare.LocalImagePath);
            //MemoryStream mem = new MemoryStream(bytes);

            //NavigationBarView.FirstNameLabel.Text = "3";
            CrossDeviceOrientation.Current.LockOrientation(CrossDeviceOrientation.Current.CurrentOrientation);
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
            //     if (!Models.Settings.Current.ShowPartBoder && IsPartAdded) return;
            float scaleX = (float)( skCanvas.Width / spareViewModel.OrgImageWidth);// / imgViewer.Bounds.Width;// * 100 / OrgImageWidth;
            float scaleY = (float)(skCanvas.Height / spareViewModel.OrgImageHeight);// / imgViewer.Bounds.Height;// * 100 / OrgImageHeight;

            float scaleXDraw = (float)(skCanvas.CanvasSize.Width / spareViewModel.OrgImageWidth);// / imgViewer.Bounds.Width;// * 100 / OrgImageWidth;
            float scaleYDraw = (float)(skCanvas.CanvasSize.Height / spareViewModel.OrgImageHeight);// / imgViewer.Bounds.Height;// * 100 / OrgImageHeight;
            
            foreach (Part part in App.analizeSpare.CSVParser.Parts)
            {


                //PartButton button = new PartButton(part);
                //button.OnLongPressed += Button_OnLongPressed;


                //button.Text = part.PartNumber;

                //absLayout.Children.Add(button, part.Rect);
                SKColor sKColor = SKColors.Blue;
                SKColor textColor = SKColors.Brown;
                if (Models.Settings.Current.ShowPartBoder)
                {
                    //   sKColor = SKColors.Transparent;
                    // textColor = SKColors.Transparent;

                    float x = (float)(part.OrgRect.Left * scaleXDraw);
                    float y =(float)(part.OrgRect.Top * scaleYDraw);
                    SKRect sKRect = new SKRect(x, y, (float)(part.OrgRect.Right * scaleXDraw), (float)(part.OrgRect.Bottom * scaleYDraw));
                    
                    _SKCanvas.DrawRect(sKRect, new SKPaint() { Color = sKColor, Style = SKPaintStyle.Stroke });
                    _SKCanvas.DrawText(part.PartNumber, new SKPoint() { X = sKRect.Left, Y = sKRect.Top }, new SKPaint() { Color = textColor, Style = SKPaintStyle.Stroke });
                }
                if (!IsPartAdded)
                    part.ReSize(scaleX, scaleY);
            }

            
                IsPartAdded = true;
            
        }
        private void DrawPartBorder(bool draw)
        {
            skCanvas.InvalidateSurface();


            //    AddButtons();
            //foreach(PartButton button in  absLayout.Children.Where(c=>c is PartButton))
            //{
            //    if (draw)
            //        button.DrawBorder();
            //    else
            //        button.RemoveBorder();
            //}
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
            //imgViewer.AddShape(ShapeType.Rectangle, new PenSettings()
            //{
            //    Color = Color.Brown,
            //    Mode = Mode.Stroke,
            //    StrokeWidth = 2,
            //    Bounds = new Rectangle(10, 10, 50, 50)
          //  });
            //imgViewer.WidthRequest = width - NavigationBarView.Width;
        }
        protected override void OnAppearing()
        {
            
            spareViewModel.OnActivate();
            base.OnAppearing();
            (Application.Current.MainPage as MainMasterDetailPage).IsGestureEnabled = false;
            //   AddButtons();
        }
        protected override void OnDisappearing()
        {
            CrossDeviceOrientation.Current.UnlockOrientation();
            (Application.Current.MainPage as MainMasterDetailPage).IsGestureEnabled = true;
            base.OnDisappearing();
            
        }
        private async void ShowCartClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartsPage());
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"Page bound (w,h) {this.Bounds.Width} -{this.Bounds.Height} ");
            //Debug.WriteLine($"Abslayout bound (w,h) {this.absLayout.Width} -{this.absLayout.Bounds.Height} ");
     //       Debug.WriteLine($"SFImageEdit bound (w,h) {this.imgViewer. Bounds.Width} -{this.imgViewer.Bounds.Height} ");
            Button btn = sender as Button;
            await App.Dialogs.AlertAsync(btn.Text);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Frame btn = sender as Frame;
            await App.Dialogs.AlertAsync(btn.Id.ToString());
        }

        private void OnPainting(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            // then we get the canvas that we can draw on
            _SKCanvas = surface.Canvas;
            
            SKBitmap sourceBitmap = SKBitmap.Decode(App.analizeSpare.LocalImagePath);
            
                Debug.WriteLine($"Image info -{sourceBitmap.Info.Width}-{sourceBitmap.Info.Height}");
            // clear the canvas / view
            _SKCanvas.Clear(SKColors.Transparent);
            SKPaint paint = new SKPaint
            {
                IsAntialias = true,
                FilterQuality = SKFilterQuality.High
            };
            _SKCanvas.DrawBitmap (sourceBitmap, e.Info.Rect,paint);
            //spareViewModel.SKViewDexX =(skCanvas.CanvasSize.Width /imgViewer.Width);
            //spareViewModel.SKViewDexY =(skCanvas.CanvasSize.Height / imgViewer.Height);
            //   if (Models.Settings.Current.ShowPartBoder)
            AddButtons();
        }
    }
}