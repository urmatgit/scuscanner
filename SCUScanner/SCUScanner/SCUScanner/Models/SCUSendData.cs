using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Models
{
    //{"ID":SCU2-01,"SN":DEV001,"C":713,"S":4278,"Apc":30,"A":2994,"W":3636,"COpc":50,"CO":2139}

    public class SCUSendData
    {
         public string ID { get; set; }
        public string SN { get; set; }
        public int C { get; set; }
        public int S { get; set; }
        public int Apc { get; set; }
        public int A { get; set; }
        public int W { get; set; }
        public int COpc { get; set; }
        public int CO { get; set; }
        public int HR { get; set; }
        public int EL { get; set; }
    }
}
