﻿using Syncfusion.XForms.TabView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    public class SfTabItemEx: SfTabItem
    {
        public Label lblTitle { get; set; }
        public int Index { get; set; }
        private SfTabView TabView;

        public SfTabItemEx(SfTabView tabView)
        {
            TabView = tabView;
            
            lblTitle = new Label();
            lblTitle.FontAttributes = FontAttributes.Bold;
            
            //lblTitle.FontSize = 20;
            lblTitle.HorizontalTextAlignment = TextAlignment.Center;
            lblTitle.VerticalTextAlignment = TextAlignment.Center;
            lblTitle.LineBreakMode = LineBreakMode.WordWrap;
            lblTitle.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command =   new Command(() => OnLabelClicked()),
            });
            
            
            HeaderContent = lblTitle;
        }

        public void OnLabelClicked(int? index=null)
        {
            TabView.SelectedIndex =index?? Index;
        }

        
        public string Caption
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }
    }
}