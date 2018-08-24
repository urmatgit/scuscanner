using SkiaSharp;
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
        public float LeftPixel { get; set; }
        public float UpperPixel { get; set; }
        public float RightPixel { get; set; }
        public float LowerPixel { get; set; }
        public SKRect Rect { get; set; }
        public SKRect OrgRect { get; set; }
        public void ReSize(float dexX, float dexY)
        {
            LeftPixel = LeftPixel * dexX;
            RightPixel = RightPixel * dexX;
            UpperPixel = UpperPixel * dexY;
            LowerPixel = LowerPixel * dexY;
            Rect = new SKRect(  LeftPixel,  UpperPixel,  RightPixel, LowerPixel);
        }
        
    }
}
