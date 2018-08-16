using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;

namespace SCUScanner.Models
{
    public class Part
    {
        public int ID { get; set; }
        public string PartName { get; set; }
        public string PartNumber { get; set; }
        public string IssueDate { get; set; }
        public int LeftPixel { get; set; }
        public int UpperPixel { get; set; }
        public int RightPixel { get; set; }
        public int LowerPixel { get; set; }
        public  Rectangle Rect { get; set; }
    }
}
