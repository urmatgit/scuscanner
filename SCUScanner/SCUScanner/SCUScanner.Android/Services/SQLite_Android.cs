using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SCUScanner.Droid.Services;
using SCUScanner.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(SQLite_Android))]
namespace SCUScanner.Droid.Services
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }
        public string GetDatabasePath(string sqliteFilename)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFilename);
            return path;
        }

        public string GetWorkDir()
        {
            var workdir= System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            workdir = Path.Combine(workdir, "manuals");
            if (!Directory.Exists(workdir))
                Directory.CreateDirectory(workdir);
            return workdir;
        }
    }
}