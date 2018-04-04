using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Helpers
{
    public interface ISQLite
    {
        string GetDatabasePath(string filename);
        string GetWorkManualDir();
    }
}
