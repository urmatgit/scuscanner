using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers.Behaviors
{
    public class MaxLengthValidatorBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);
        public static readonly BindableProperty ErrorLabelProperty = BindableProperty.Create("ErrorLabel", typeof(string), typeof(BetweenValueValidatorBehavior), "");
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public string ErrorLabel
        {
            get { return (string)GetValue(ErrorLabelProperty); }
            set { SetValue(ErrorLabelProperty, value); }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += bindable_TextChanged;
        }

        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsValid = e.NewTextValue.Length!=MaxLength;
            Label errorLabel = ((Entry)sender).FindByName<Label>(ErrorLabel);
            ((Entry)sender).TextColor = !IsValid ? Color.Default : Color.Red;
            if (e.NewTextValue.Length >= MaxLength)
                ((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength);
            if (errorLabel != null)
            {
                if (IsValid)
                {
                    errorLabel.Text = string.Format(Settings.Current.Resources["LengthMustBeText"], MaxLength);
                }
                else
                {
                    errorLabel.Text = "";
                }
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;

        }
    }
}
