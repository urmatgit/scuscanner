using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Models
{
    public class DevicesItem
    {
        public int id { get; set; }
        public string ImageInfo { get; private set; }
        public string UnitName { get; set; }
        public string SerialNo { get; set; }
        public DevicesItem()
        {
            ImageInfo = "information.png";
        }
    }
}
