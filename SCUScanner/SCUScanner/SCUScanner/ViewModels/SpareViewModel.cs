using Acr.UserDialogs;
using MR.Gestures;
using ReactiveUI;
using SCUScanner.Helpers;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class SpareViewModel: BaseViewModel
    {
        public double OrgImageWidth { get; set; } = 4958;
        public double OrgImageHeight { get; set; } = 7015;
        public double SKViewDexX { get; set; } = 1;
        public double SKViewDexY { get; set; } = 1;

        public ICommand AddCommand { get; }
        public ICommand DownCommand { get; protected set; }
        public ICommand UpCommand { get; protected set; }
        public ICommand TappingCommand { get; protected set; }
        public ICommand TappedCommand { get; protected set; }
        public ICommand DoubleTappedCommand { get; protected set; }
        public ICommand LongPressingCommand { get; protected set; }
        public ICommand LongPressedCommand { get; protected set; }
        public ICommand PinchingCommand { get; protected set; }
        public ICommand PinchedCommand { get; protected set; }
        public ICommand PanningCommand { get; protected set; }
        public ICommand PannedCommand { get; protected set; }
        public ICommand SwipedCommand { get; protected set; }
        public ICommand RotatingCommand { get; protected set; }
        public ICommand RotatedCommand { get; protected set; }
        private string cartCount="";
        public string CartCount
        {
            get => cartCount;
            set => this.RaiseAndSetIfChanged(ref cartCount, value);
        }
        private ImageSource imageSpare;
        public ImageSource ImageSpare
        {
            get =>   imageSpare; 
            set=> this.RaiseAndSetIfChanged(ref imageSpare, value);
        }
        public SpareViewModel()
        {
            //ImageSpare = ImageSource.FromFile(App.analizeSpare.LocalImagePath);
            AddCommand = ReactiveCommand.Create(() =>
            {
            var rnd = new Random();
            App.analizeSpare.vmCarts.AddCart(App.analizeSpare.CSVParser.Parts[rnd.Next(0, App.analizeSpare.CSVParser.Parts.Count - 1)]);
                CartCount = App.analizeSpare.vmCarts.TotalSum().ToString();
            });


            DownCommand = new Command<DownUpEventArgs>(OnDown);
            UpCommand = new Command<DownUpEventArgs>(OnUp);
            TappingCommand = new Command<TapEventArgs>(OnTapping);
            TappedCommand = new Command<TapEventArgs>(OnTapped);
            DoubleTappedCommand = new Command<TapEventArgs>(OnDoubleTapped);
            LongPressingCommand = new Command<LongPressEventArgs>(OnLongPressing);
            LongPressedCommand = new Command<LongPressEventArgs>(OnLongPressed);
            PinchingCommand = new Command<PinchEventArgs>(OnPinching);
            PinchedCommand = new Command<PinchEventArgs>(OnPinched);
            PanningCommand = new Command<PanEventArgs>(OnPanning);
            PannedCommand = new Command<PanEventArgs>(OnPanned);
            SwipedCommand = new Command<SwipeEventArgs>(OnSwiped);

            //if (App.analizeSpare.ErrorConnect) // пока временно!!!
            //    ImageSpare = ImageSource.FromResource(App.analizeSpare.LocalImagePath);
            //else

            var size = DependencyService.Get<ISQLite>().GetImageOrgSize(App.analizeSpare.LocalImagePath);
            OrgImageHeight = size.Height;
            OrgImageWidth = size.Width;
            
                GC.Collect();
            GC.Collect();
            ImageSpare = ImageSource. FromFile(App.analizeSpare.LocalImagePath);
            App.Dialogs.Toast($"Loaded {App.analizeSpare.LocalImagePath}");

        }

        private void OnSwiped(SwipeEventArgs obj)
        {
            var e = obj;
        }

        private void OnLongPressed(LongPressEventArgs obj)
        {
            // AddCommand.Execute(null);
        }

        private void OnLongPressing(LongPressEventArgs obj)
        {
            var e = obj;
        }

        private void OnDoubleTapped(TapEventArgs obj)
        {
            Scale = 1;
            TranslationX = 0;
            TranslationY = 0;
        }

        private void OnTapping(TapEventArgs obj)
        {
            var e = obj;
        }

        private void OnUp(DownUpEventArgs obj)
        {
            var e = obj;
        }

        private void OnDown(DownUpEventArgs obj)
        {
            var e = obj;
        }

        public override void OnActivate(string kod = "")
        {
            CartCount = App.analizeSpare.vmCarts.TotalSum().ToString();
            base.OnActivate(kod);
        }

        #region Properties

        protected double anchorX = 0.5;
        public double AnchorX
        {
            get { return anchorX; }
            set { this.RaiseAndSetIfChanged(ref anchorX, value); }
        }

        protected double anchorY = 0.5;
        public double AnchorY
        {
            get { return anchorY; }
            set { this.RaiseAndSetIfChanged(ref anchorY, value); }
        }

        protected double rotation = 0;
        public double Rotation
        {
            get { return rotation; }
            set { this.RaiseAndSetIfChanged(ref rotation, value); }
        }

        protected double scale = 1;
        public double Scale
        {
            get { return scale; }
            set { this.RaiseAndSetIfChanged(ref scale, value); }
        }

        protected double translationX = 0;
        public double TranslationX
        {
            get { return translationX; }
            set { this.RaiseAndSetIfChanged(ref translationX, value); }
        }

        protected double translationY = 0;
        public double TranslationY
        {
            get { return translationY; }
            set { this.RaiseAndSetIfChanged(ref translationY, value); }
        }

        public double ViewX { get; set; }
        public double ViewY { get; set; }
        public double ViewWidth { get; set; }
        public double ViewHeight { get; set; }

        #endregion

        protected  void OnTapped(MR.Gestures.TapEventArgs e)
        {

            var obj = e;
            double dexX = (e.ViewPosition.Width * Scale - e.ViewPosition.Width) / 2;
            double dexY = (e.ViewPosition.Height * Scale - e.ViewPosition.Height) / 2;
            double x = e.Touches[0].X + dexX - TranslationX;
            double y = e.Touches[0].Y + dexY - TranslationY;
            var parts = App.analizeSpare.CheckContain(x, y);
            if (parts.Length>0)
            {
                SelectPart(parts);
            }
        }
        private void SelectPart(Part[] parts)
        {
            var actions = new ActionSheetConfig()
                .SetTitle($"{Resources["SelectPartText"]}");

            foreach (Part part in parts)
            {
                actions.Add($"{part.PartNumber},{part.PartName}",()=>
                {
                    App.analizeSpare.vmCarts.AddCart(part);
                    CartCount = App.analizeSpare.vmCarts.TotalSum().ToString();
                });
            }
            actions.SetCancel(Models.Settings.Current.Resources["CancelText"]);

            App.Dialogs.ActionSheet(actions);
               

        }
        protected   void OnPanned(MR.Gestures.PanEventArgs e)
        {

            var obj = e;

            var x= TranslationX;
            var y =TranslationY;
            
        }
        protected   void OnPinched(MR.Gestures.PinchEventArgs e)
        {
           

            SetAnchor(e.Center);
            var newScale = Scale * e.DeltaScale;
            Scale = Math.Min(5, Math.Max(0.1, newScale));
        }

        protected   void OnPanning(MR.Gestures.PanEventArgs e)
        {

         
            TranslationX += e.DeltaDistance.X;
            TranslationY += e.DeltaDistance.Y;

            double dexX = (e.ViewPosition.Width * Scale - e.ViewPosition.Width)/2;
            double dexY = (e.ViewPosition.Height * Scale - e.ViewPosition.Height)/2;
            double dexY1 = (e.ViewPosition.Y * Scale - e.ViewPosition.Y)/2;
            if (TranslationX < dexX*-1)
                TranslationX = dexX * -1;
            //if (Scale==1 && TranslationX < e.ViewPosition.X)
            //    TranslationX = e.ViewPosition.X;
            if (TranslationY   <dexY1+ dexY*-1)
                TranslationY = dexY1+ dexY * -1;

            if (TranslationX + e.ViewPosition.Width > e.ViewPosition.Width+dexX)
            {
                TranslationX = dexX;
            }
            if ( TranslationY + e.ViewPosition.Height >e.ViewPosition.Height+ dexY-dexY1 )
            {
                TranslationY =  dexY-dexY1  ;
            }

        }
        protected   void OnPinching(MR.Gestures.PinchEventArgs e)
        {
          

            SetAnchor(e.Center);
            var newScale = Scale * e.DeltaScale;
            Scale = Math.Min(5, Math.Max(0.1, newScale));
            if (Scale < 1)
            {
                Scale = 1;
                TranslationX = 0;
                TranslationY = 0;
            }
        }

        protected void SetAnchor(Point center)
        {
            // in AnchorX/Y 0.0 means the top left corner and 1.0 means the bottom right
            // unfortunately I don't know how to calculate this correct if Translation, Scale and Rotation is used

            //var xWithinView = center.X - ViewX;
            //var yWithinView = center.Y - ViewY;
            //AnchorX = ViewWidth / xWithinView;
            //AnchorY = ViewHeight / yWithinView;
        }
    }
}
