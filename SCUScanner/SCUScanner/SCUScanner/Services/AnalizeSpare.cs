using FluentFTP;
using SCUScanner.Helpers;
using SCUScanner.Models;
using Spares.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SCUScanner.Services
{
   public class AnalizeSpare
    {
        private const string CSVPath = "CSV";
        private const string EmailsPath = "emails";
        private const string ThumpPath = "thump";
        private const string FtpHost = "ftp://ftp.chester.ru";
        CSVParser CSVParser;
        private List<Cart> Carts { get; set; } = new List<Cart>();
        string WorkDir;
        string RootPath { get;  set; }
        string _serialnumber { get; set; }
        public  AnalizeSpare(string rootpath, string serialnumber)
        {
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            RootPath = rootpath;
            _serialnumber = serialnumber;
            ReadCSV();

        }
        private async void ReadCSV()
        {
            string path = await DownLoad(RootPath,$"{GetFileNameFromSerialNo(_serialnumber)}.csv");
            string emailpath = await DownLoad(RootPath, $"brandsalesemails.csv");
            CSVParser = new CSVParser(path, emailpath);
        }
        public Part[] CheckContain(int x,int y)
        {
            return  CSVParser.CheckContainInRect(x, y);
        }
        public void UpdateCarts(List<Cart> carts)
        {
            this.Carts = carts;
        }
        public void AddCart(Part part)
        {
            var finded = Carts.FirstOrDefault(c => c.Part.PartName == part.PartName);
            if (finded == null)
            {
                var cart = new Cart();
                cart.Part = part;
                Carts.Add(cart);
            }
            else
                finded.Count++;

        }
        
        public void ClearCarts()
        {
            Carts.Clear();

        }
        public string GetEmailTo()
        {
            string bb = GetFileNameFromSerialNo(_serialnumber);
                bb= bb.Substring(bb.Length - 2);
            string email = CSVParser.Emails.FirstOrDefault(e => e.BB == bb)?.email;
            if (string.IsNullOrEmpty(email))
            {
                return $"not fount email by {bb}";
            }
            else

                return email;


        }
        public string GenerateResultSTR(List<Cart> carts)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ///TODO translate
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"Spare Parts {_serialnumber}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Do not edit this section");
            stringBuilder.AppendLine("-".PadRight(100));
            //
            foreach(Cart cart in carts)
            {
                stringBuilder.AppendLine($"{cart.Count},{cart.Part.PartNumber},{cart.Part.PartName}");
            }
            stringBuilder.AppendLine("-".PadRight(100));
            stringBuilder.AppendLine("Please supply your contact number below");
            return stringBuilder.ToString();
        }
        private string GetFileNameFromSerialNo(string serial)
        {
            return serial.Substring(1, 15);
            //string result = "";
            //int index_ = serial.LastIndexOf('-');
            //if (index_ == 0)
            //    index_ = serial.Length;
            //result = serial.Substring(0, index_).ToLower();
            //result = $"{result}({SettingsBase.SelectedLang.ToLower()}).pdf";
            //return result;
        }
        #region FTP
        private async Task<string> DownLoad(string path,string filename)
        {
            var filenamepath = Path.Combine(WorkDir,path);
            if (!Directory.Exists(filenamepath))
            {
                Directory.CreateDirectory(filenamepath);
            }

            var remotefilename=$"{path}/{filename}";
            FtpClient client = new FtpClient(FtpHost);
            try
            {

                client.Credentials = new NetworkCredential("chesterr_urmat", "Scuscanner2018");

                // begin connecting to the server
                await client.ConnectAsync();
                if (client.IsConnected)
                {


                    {

                        if (client.FileExists($"{remotefilename}"))
                        {
                            //if (File.Exists(filename))
                            //{
                            //    File.Delete(filename);
                            //}
                            bool dowloaded = false;
                            try
                            {
                                using (var cancelSrc = new CancellationTokenSource())
                                {
                                    using (App.Dialogs.Loading(Settings.Current.Resources["DownloadText"], cancelSrc.Cancel, Settings.Current.Resources["CancelText"]))
                                    {
                                        string tmpFileName = Path.Combine(filenamepath, filename+ DateTime.Now.Second.ToString());
                                        dowloaded = await client.DownloadFileAsync(tmpFileName, $"{remotefilename}", true);
                                        filenamepath = Path.Combine(filenamepath, filename);
                                        File.Copy(tmpFileName, filenamepath, true);
                                        try
                                        {
                                            File.Delete(tmpFileName);
                                        }
                                        catch (Exception er)
                                        {

                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                await App.Dialogs.AlertAsync(ex.ToString());

                            }
                            //if (dowloaded || File.Exists(filename))
                            //{
                            //    WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
                            //    await Navigation.PushAsync(webViewPageCS);
                            //}

                        }
                        else
                        {

                            await App.Dialogs.AlertAsync(Settings.Current.Resources["ManualNotFoundText"]);
                        }



                    };
                }
                else
                {
                    await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
                }
            }
            catch (Exception ex)
            {
                await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
                filenamepath = "";
            }
            finally
            {
                await client?.DisconnectAsync();
            }
            return filenamepath;
        }
        #endregion
    }
}
