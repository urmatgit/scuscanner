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
        public double LeftPixel { get; set; }
        public double UpperPixel { get; set; }
        public double RightPixel { get; set; }
        public double LowerPixel { get; set; }
        public  Rectangle Rect { get; set; }
        public Rectangle OrgRect { get; set; }
        public void ReSize(double dexX, double dexY)
        {
            LeftPixel = LeftPixel * dexX;
            RightPixel = RightPixel * dexX;
            UpperPixel = UpperPixel * dexY;
            LowerPixel = LowerPixel * dexY;
            Rect = new Xamarin.Forms.Rectangle(LeftPixel, UpperPixel, RightPixel, LowerPixel);
        }
        
    }
}
