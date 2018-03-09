using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class DefaultStyle : DataGridStyle
    {
        public DefaultStyle()
        {
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromHex("#e0e0e0");
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromHex("#b2d8f7");
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.Black;
        }

        public override Color GetRecordForegroundColor()
        {
            return Color.Black;
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromHex("#e6e6e6");
        }

        public override Color GetRecordBackgroundColor()
        {
            return Color.White;
        }
        public override ImageSource GetGroupCollapseIcon()
        {
            return null;
        }
        public override ImageSource GetHeaderSortIndicatorDown()
        {
            return null;
        }
    }

    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class SelectionStyle : DataGridStyle
    {
        public SelectionStyle()
        {
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromHex("#e0e0e0");
        }

        public override Color GetRecordForegroundColor()
        {
            return Color.Black;
        }

        public override Color GetRecordBackgroundColor()
        {
            return Color.White;
        }
    }

    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class CellTemplateStyle : DataGridStyle
    {
        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromHex("#cce5fa");
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.Black;
        }

    }
}
