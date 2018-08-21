
using MR.Gestures;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace SCUScanner.Helpers
{



   public class PartButton:Button
    {
        public event EventHandler OnClicked;
        public event EventHandler OnLongPressed;
        public Part Part { get; set; }
        public PartButton(Part part)
        {
            this.Part = part;
           
            BackgroundColor =Xamarin.Forms.Color.Transparent;
            BorderColor = Xamarin.Forms.Color.Gray;
            BorderWidth = 1;
            this.Clicked += PartButton_Clicked;
            this.LongPressed += PartButton_LongPressed;
        }

        private void PartButton_LongPressed(object sender, LongPressEventArgs e)
        {
            OnLongPressed?.Invoke(this, e);
        }

        private void PartButton_Clicked(object sender, EventArgs e)
        {
            OnClicked?.Invoke(this, e);
        }
    }
}
