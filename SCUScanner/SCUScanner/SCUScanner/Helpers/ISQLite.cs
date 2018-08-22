using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    public interface ISQLite
    {
        string GetDatabasePath(string filename);
        string GetWorkManualDir();
        Size GetImageOrgSize(string path);
    }
}
