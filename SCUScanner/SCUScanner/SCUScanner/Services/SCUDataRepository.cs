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
        public async Task<List<DevicesItem>> GetDevicesItem()
        {
#if DEBUG
            var devList = new List<DevicesItem>();
            devList.Add(new DevicesItem() { id = 1, SerialNo = "0001", UnitName = "test1" });
            devList.Add(new DevicesItem() { id = 2, SerialNo = "0002", UnitName = "test2" });
            devList.Add(new DevicesItem() { id = 3, SerialNo = "0003", UnitName = "test3" });
            return devList; 
#else
            return await database.QueryAsync<DevicesItem>("Select id, UnitName, SerialNo from SCUItem group by UnitName,SerialNo order by UnitName", "");
#endif
        }
        public async Task<SCUItem> GetItemAsync(int id)
        {
            return await database.GetAsync<SCUItem>(id);
        }
        public async Task<List<SCUItem>> GetItemAsync(string unitname)
        {
            return await database.Table<SCUItem>().Where(s => s.UnitName.Trim() == unitname).ToListAsync();
        }
        public  async  Task<int> GetItemAsyncCount(string unitname)
        {
            return await database.ExecuteScalarAsync<int>($"select count(*) from SCUItem where UnitName like  '{unitname}'");
        }
        public async Task<int> GetItemAsyncCount(string unitname,string sn)
        {
            return await database.ExecuteScalarAsync<int>($"select count(*) from SCUItem where UnitName like  '{unitname}' and SerialNo like '{sn}'");
        }
        public async Task<List<SCUItem>> GetItemAsync(string unitname,int start=0,int rowcount=5)
        {
            
            return await database.QueryAsync<SCUItem>($"Select * from SCUItem where UnitName like '{unitname}' order by id DESC limit {start},{rowcount}");
        }
        public async Task<List<SCUItem>> GetItemAsync(string unitname,string sn, int start = 0, int rowcount = 5)
        {

            return await database.QueryAsync<SCUItem>($"Select * from SCUItem where UnitName like '{unitname}' and  SerialNo like '{sn}' order by id DESC limit {start},{rowcount}");
        }
        public async Task<int> DeleteItemAsync(SCUItem item)
        {
            return await database.DeleteAsync(item);
        }
        public async Task<int> DeleteItemAsync(int id)
        {
            return await database.ExecuteScalarAsync<int>("delete from SCUItem where id=?", id);
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

