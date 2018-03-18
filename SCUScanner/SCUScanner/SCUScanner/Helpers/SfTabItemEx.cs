using Syncfusion.XForms.TabView;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    public class SfTabItemEx: SfTabItem
    {
        public Label lblTitle { get; set; }
        public SfTabItemEx()
        {
            lblTitle = new Label();
            lblTitle.FontAttributes = FontAttributes.Bold;
            //lblTitle.FontSize = 20;
            lblTitle.HorizontalTextAlignment = TextAlignment.Center;
            lblTitle.VerticalTextAlignment = TextAlignment.Center;
            lblTitle.LineBreakMode = LineBreakMode.WordWrap;
            lblTitle.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnLabelClicked()),
            });
            HeaderContent = lblTitle;
        }

        private void OnLabelClicked()
        {
            
        }

        
        public string Caption
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }
    }
}
