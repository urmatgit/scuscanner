﻿using Cirrious.FluentLayouts.Touch;
using SCUScanner.Helpers;
using SCUScanner.iOS.Controls;
using System;
using Foundation;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(PlaceholderEditor), typeof(PlaceholderEditorRenderer))]
namespace SCUScanner.iOS.Controls
{
    public class PlaceholderEditorRenderer : EditorRenderer
    {
        private UILabel _placeholderLabel;

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
                return;

            CreatePlaceholderLabel((PlaceholderEditor)Element, Control);

            Control.Ended += OnEnded;
            Control.Changed += OnChanged;
            
        }

        private void CreatePlaceholderLabel(PlaceholderEditor element, UITextView parent)
        {
            _placeholderLabel = new UILabel
            {
                Text = element.Placeholder,
                TextColor = element.PlaceholderColor.ToUIColor(),
                BackgroundColor = UIColor.Clear,
                Font = UIFont.SystemFontOfSize((nfloat)element.FontSize)
            };
            _placeholderLabel.SizeToFit();

            parent.AddSubview(_placeholderLabel);

            parent.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            parent.AddConstraints(
                _placeholderLabel.AtLeftOf(parent, 7),
                _placeholderLabel.WithSameCenterY(parent)
            );
            parent.LayoutIfNeeded();

            _placeholderLabel.Hidden = parent.HasText;
        }

        private void OnEnded(object sender, EventArgs args)
        {
            if (!((UITextView)sender).HasText && _placeholderLabel != null)
                _placeholderLabel.Hidden = false;
        }

        private void OnChanged(object sender, EventArgs args)
        {
            if (_placeholderLabel != null)
                _placeholderLabel.Hidden = ((UITextView)sender).HasText;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Ended -= OnEnded;
                Control.Changed -= OnChanged;

                _placeholderLabel?.Dispose();
                _placeholderLabel = null;
            }

            base.Dispose(disposing);
        }
    }
}
