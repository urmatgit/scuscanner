using ReactiveUI;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SCUScanner.ViewModels
{
    public class SCUDatasViewModel : BaseViewModel
    {
        internal delegate void FilterChanged();

        internal FilterChanged filtertextchanged;
        private string selectedcolumn = "All Columns";
        public string SelectedColumn
        {
            get { return selectedcolumn; }
            set { selectedcolumn = value; }
        }
        private string selectedcondition = "Contains";
        public string SelectedCondition
        {
            get { return selectedcondition; }
            set { selectedcondition = value; }
        }
        private string filtertext = "";
        public string FilterText
        {
            get { return filtertext; }
            set
            {
                filtertext = value;
                OnFilterTextChanged();
                this.RaiseAndSetIfChanged(ref filtertext, value);

            }

        }
        private bool _VisibleOptionList = false;
        public bool VisibleOptionList
        {
            get => _VisibleOptionList;
            set => this.RaiseAndSetIfChanged(ref _VisibleOptionList, value);
        }
   
        private void OnFilterTextChanged()
        {
            if (filtertextchanged != null)
                filtertextchanged();
        }
        public bool FilerRecords(object o)
        {
            double res;
            bool checkNumeric = double.TryParse(FilterText, out res);
            var item = o as SCUItem;
            if (item != null && FilterText.Equals("") && !string.IsNullOrEmpty(FilterText))
            {
                return true;
            }
            else
            {
                if (item != null)
                {
                    if (checkNumeric && !SelectedColumn.Equals("All Columns") && !SelectedCondition.Equals("Contains"))
                    {
                        bool result = MakeNumericFilter(item, SelectedColumn, SelectedCondition);
                        return result;
                    }
                    else if (SelectedColumn.Equals("All Columns"))
                    {
                        if (item.ID?.ToLower().Contains(FilterText.ToLower()) ==true ||
                            item.HoursElapsed.ToString().ToLower().Contains(FilterText.ToLower()) ||
                            item.AlarmHours .ToString().ToLower().Contains(FilterText.ToLower()) ||
                            item.AlarmSpeed.ToString().ToLower().Contains(FilterText.ToLower()) ||
                            item.DateWithTime.ToString().ToLower().Contains(FilterText.ToLower()) ||
                            item.Location?.ToString().ToLower().Contains(FilterText.ToLower())==true ||
                            item.Notes?.ToString().ToLower().Contains(FilterText.ToLower())==true ||
                            item.Operator?.ToString().ToLower().Contains(FilterText.ToLower()) ==true ||
                            item.SerialNo?.ToString().ToLower().Contains(FilterText.ToLower()) ==true ||
                            item.Speed.ToString().ToLower().Contains(FilterText.ToLower()) ||
                            item.BroadCastId?.ToString().ToLower().Contains(FilterText.ToLower())==true)
                            return true;
                        return false;
                    }
                    else
                    {
                        bool result = MakeStringFilter(item, SelectedColumn, SelectedCondition);
                        return result;
                    }
                }
            }
            return false;
        }
        private bool MakeStringFilter(SCUItem o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            
            var exactValue = value?.GetValue(o, null);
            exactValue = exactValue.ToString().ToLower();
            string text = FilterText.ToLower();
            var methods = typeof(string).GetMethods();

            if (methods.Count() != 0)
            {
                if (condition == "Contains")
                {
                    var methodInfo = methods.FirstOrDefault(method => method.Name == condition);
                    bool result1 = (bool)methodInfo.Invoke(exactValue, new object[] { text });
                    return result1;
                }
                else if (exactValue.ToString() == text.ToString())
                {
                    bool result1 = String.Equals(exactValue.ToString(), text.ToString());
                    if (condition == "Equals")
                        return result1;
                    else if (condition == "NotEquals")
                        return false;
                }
                else if (condition == "NotEquals")
                {
                    return true;
                }
                return false;
            }
            else
                return false;
        }
        private bool MakeNumericFilter(SCUItem o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value.GetValue(o, null);
            double res;
            bool checkNumeric = double.TryParse(exactValue.ToString(), out res);
            if (checkNumeric)
            {
                switch (condition)
                {
                    case "Equals":
                        try
                        {
                            if (exactValue.ToString() == FilterText)
                            {
                                if (Convert.ToDouble(exactValue) == (Convert.ToDouble(FilterText)))
                                    return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                        break;
                    case "NotEquals":
                        try
                        {
                            if (Convert.ToDouble(FilterText) != Convert.ToDouble(exactValue))
                                return true;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        public ObservableCollection<SCUItem>  SCUItems { get; private set; }
        public SCUDatasViewModel()
        {
            SCUItems = new ObservableCollection<SCUItem>();
            //    FillItems();
            FillItems();
        }
        public override void OnActivate()
        {
            base.OnActivate();
            FillItems();

        }
        private async void FillItems()
        {
            var list = await App.Database.GetItemsAsync();
            try
            {
                list.ForEach(l => SCUItems.Add(l));
                //SCUItems.  = new ObservableCollection<SCUItem>(list);
            }catch(Exception er)
            {
                await App.Dialogs.AlertAsync(er.Message);
            }
            //SCUItems.Add(new SCUItem()
            //{
            //    ID = "SCU2-01",
            //    DateWithTime = DateTime.Now,
            //    Location= "DEV001",
            //    MacAddress= "28:47:76:10",
            //    Operator="User",
            //    Comment= "Text string (255 chars max?)",
            //    Speed=100
            //});
        }
    }
}
