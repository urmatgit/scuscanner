using SCUScanner.Helpers;
using SCUScanner.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SCUScanner.Services
{
   public class SCUDataRepository
    {
        SQLiteAsyncConnection database;

        public SCUDataRepository(string filename)
        {
            string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            database = new SQLiteAsyncConnection(databasePath);
        }

        public async Task CreateTable()
        {
            await database.CreateTableAsync<SCUItem>();
        }
        public async Task<List<SCUItem>> GetItemsAsync()
        {
            return await database.Table<SCUItem>().ToListAsync();

        }
        public async Task<SCUItem> GetItemAsync(int id)
        {
            return await database.GetAsync<SCUItem>(id);
        }
        public async Task<int> DeleteItemAsync(SCUItem item)
        {
            return await database.DeleteAsync(item);
        }
        public async Task<int> SaveItemAsync(SCUItem item)
        {
            if (item.Id != 0)
            {
                await database.UpdateAsync(item);
                return item.Id;
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }
    }
}

