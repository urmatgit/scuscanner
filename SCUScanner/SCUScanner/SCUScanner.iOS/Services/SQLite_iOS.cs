using SCUScanner.Helpers;
using SCUScanner.iOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

//[assembly: Dependency(typeof(SQLite_iOS))]
[assembly: Xamarin.Forms.Dependency(typeof(SQLite_iOS))]
namespace SCUScanner.iOS.Services
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS() { }
        public string GetDatabasePath(string sqliteFilename)
        {
            // определяем путь к бд
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // папка библиотеки
            var path = Path.Combine(libraryPath, sqliteFilename);

            return path;
        }

        public string GetWorkManualDir()
        {
            var workdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            workdir = Path.Combine(workdir, "manuals");
            if (!Directory.Exists(workdir))
                Directory.CreateDirectory(workdir);
            return workdir;
             
        }

        
    }
}
