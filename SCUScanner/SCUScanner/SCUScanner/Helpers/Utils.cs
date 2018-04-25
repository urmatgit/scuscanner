using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Helpers
{
   public static class Utils
    {
        public static string GetFileNameFromSerialNo(string serial,string lang)
        {
            string result = "";
            int index_ = serial.LastIndexOf('-');
            if (index_ == 0)
                index_ = serial.Length;
            result = serial.Substring(0, index_).ToLower();
            result = $"{result}({lang}).pdf";
            return result;
        }
    }
}
