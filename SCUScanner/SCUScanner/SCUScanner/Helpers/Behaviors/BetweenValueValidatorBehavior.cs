using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers.Behaviors
{
   public class BetweenValueValidatorBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create("MaxValue", typeof(int), typeof(BetweenValueValidatorBehavior), 0);
        public static readonly BindableProperty MinValueProperty = BindableProperty.Create("MinValue", typeof(int), typeof(BetweenValueValidatorBehavior), 0);
        public static readonly BindableProperty ErrorLabelProperty = BindableProperty.Create("ErrorLabel", typeof(string), typeof(BetweenValueValidatorBehavior), "");
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
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
            int value = 0;
            bool IsValid = false;
            Label errorLabel = ((Entry)sender).FindByName<Label>(ErrorLabel);
            if (int.TryParse(e.NewTextValue,out value))
            {
               IsValid = value < MinValue || value > MaxValue;
            }else if (string.IsNullOrEmpty(e.NewTextValue))
            {
                IsValid = false;
            }
            ((Entry)sender).TextColor = !IsValid ? Color.Default : Color.Red;
            if (errorLabel != null)
            {
                if (IsValid)
                {
                    errorLabel.Text = string.Format(Settings.Current.Resources["RangeWillBeValueText"], MinValue, MaxValue);
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

