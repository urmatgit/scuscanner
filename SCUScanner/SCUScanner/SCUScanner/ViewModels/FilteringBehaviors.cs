using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class FilteringBehaviors : Behavior<ContentPage>
    {
        private Syncfusion.SfDataGrid.XForms.SfDataGrid dataGrid;
        private SCUDatasViewModel viewModel;
        private Picker optionsList;
        private Picker columnsList;
        private SearchBar filterText;

        protected override void OnAttachedTo(ContentPage bindable)
        {
            viewModel = new SCUDatasViewModel();
            dataGrid = bindable.FindByName<Syncfusion.SfDataGrid.XForms.SfDataGrid>("dataGrid");
            bindable.BindingContext = viewModel;
            optionsList = bindable.FindByName<Picker>("OptionsList");
            columnsList = bindable.FindByName<Picker>("ColumnsList");
            filterText = bindable.FindByName<SearchBar>("filterText");
            optionsList.Items.Add(viewModel.Resources["EqualsText"]);//"Equals");
            optionsList.Items.Add(viewModel.Resources["NotEquals"]);// "NotEquals");
            optionsList.Items.Add(viewModel.Resources["Contains"]);//"Contains");
            columnsList.Items.Add(viewModel.Resources["AllColumnsText"]);//"All Columns");

            columnsList.Items.Add("ID");
            columnsList.Items.Add("SerialNo");
            columnsList.Items.Add("BroadCastId");
            columnsList.Items.Add("Location");
            columnsList.Items.Add("Notes");
            columnsList.Items.Add("Speed");
            columnsList.Items.Add("Operator");
            columnsList.Items.Add("AlarmSpeed");
            columnsList.Items.Add("DateWithTime");
            columnsList.Items.Add("HoursElapsed");
            columnsList.Items.Add("AlarmHours");

            columnsList.SelectedIndex = 0;
            viewModel.filtertextchanged = OnFilterChanged;
            filterText.TextChanged += OnFilterTextChanged;
            columnsList.SelectedIndexChanged += OnColumnsSelectionChanged;
            optionsList.SelectedIndexChanged += OnFilterOptionsChanged;
            base.OnAttachedTo(bindable);
        }
        public void OnColumnsSelectionChanged(object sender, EventArgs e)
        {
            Picker newPicker = (Picker)sender;
            viewModel.SelectedColumn = newPicker.Items[newPicker.SelectedIndex];
            if (viewModel.SelectedColumn == viewModel.Resources["AllColumnsText"])//"All Columns")
            {
                viewModel.SelectedCondition = viewModel.Resources["Contains"];// "Contains";
                optionsList.IsVisible = false;
                this.OnFilterChanged();
            }
            else
            {
                optionsList.IsVisible = true;
                foreach (var prop in typeof(SCUItem).GetProperties())
                {
                    if (prop.Name == viewModel.SelectedColumn)
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            optionsList.Items.Clear();
                            optionsList.Items.Add(viewModel.Resources["ContainsText"]); //("Contains");
                            optionsList.Items.Add(viewModel.Resources["EqualsText"]); //"Equals");
                            optionsList.Items.Add(viewModel.Resources["NotEqualsText"]); //"NotEquals");
                            if (this.viewModel.SelectedCondition == viewModel.Resources["EqualsText"])
                                optionsList.SelectedIndex = 1;
                            else if (this.viewModel.SelectedCondition == viewModel.Resources["NotEqualsText"])
                                optionsList.SelectedIndex = 2;
                            else
                                optionsList.SelectedIndex = 0;
                        }
                        else
                        {
                            optionsList.Items.Clear();
                            optionsList.Items.Add(viewModel.Resources["EqualsText"]); //("Equals");
                            optionsList.Items.Add(viewModel.Resources["NotEqualsText"]); //"NotEquals");
                            if (this.viewModel.SelectedCondition == viewModel.Resources["EqualsText"])
                                optionsList.SelectedIndex = 0;
                            else
                                optionsList.SelectedIndex = 1;
                        }
                    }
                }
            }
        }

        public void OnFilterOptionsChanged(object sender, EventArgs e)
        {
            Picker newPicker = (Picker)sender;
            if (newPicker.SelectedIndex >= 0)
            {
                viewModel.SelectedCondition = newPicker.Items[newPicker.SelectedIndex];
                if (filterText.Text != null)
                    this.OnFilterChanged();
            }
        }

        public void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == null)
                viewModel.FilterText = "";
            else
                viewModel.FilterText = e.NewTextValue;
        }

        public void OnFilterChanged()
        {
            if (dataGrid.View != null)
            {
                dataGrid.View.Filter = viewModel.FilerRecords;
                dataGrid.View.RefreshFilter();
            }
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            optionsList.SelectedIndexChanged -= OnFilterOptionsChanged;
            columnsList.SelectedIndexChanged -= OnColumnsSelectionChanged;
            filterText.TextChanged -= OnFilterTextChanged;
            dataGrid = null;
            optionsList = null;
            columnsList = null;
            filterText = null;
            base.OnDetachingFrom(bindable);
        }

    }
}
