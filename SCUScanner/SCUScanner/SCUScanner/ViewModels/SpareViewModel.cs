using MR.Gestures;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class SpareViewModel: BaseViewModel
    {
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
            
            

        }

        private void OnSwiped(SwipeEventArgs obj)
        {
            var e = obj;
        }

        private void OnLongPressed(LongPressEventArgs obj)
        {
            var e = obj;
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
            //TranslationX += e.ViewPosition.X;
            //TranslationY += e.ViewPosition.Y;
        }
        protected   void OnPanned(MR.Gestures.PanEventArgs e)
        {

            var obj = e;
            //TranslationX += e.DeltaDistance.X;
            //TranslationY += e.DeltaDistance.Y;
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
            if (Scale==1 && TranslationX < e.ViewPosition.X)
                TranslationX = e.ViewPosition.X;
            if (Scale == 1 && (e.ViewPosition.Y+TranslationY) < e.ViewPosition.Y)
                TranslationY = 0;
            if (Scale == 1 &&  TranslationX + e.ViewPosition.Width > e.ViewPosition.Width)
            {
                TranslationX = 0;
            }
            if (Scale == 1 && TranslationY + e.ViewPosition.Height > e.ViewPosition.Height)
            {
                TranslationY = 0;
            }

        }
        protected   void OnPinching(MR.Gestures.PinchEventArgs e)
        {
          

            SetAnchor(e.Center);
            var newScale = Scale * e.DeltaScale;
            Scale = Math.Min(5, Math.Max(0.1, newScale));
          if (Scale < 1) Scale = 1;
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
