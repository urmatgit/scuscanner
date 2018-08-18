using Acr.UserDialogs;
using FluentFTP;
using SCUScanner.Helpers;
using SCUScanner.Models;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string CSVPath = "csv";
        private const string EmailsPath = "brandemails";
        private const string ThumpPath = "thumbnails";
        private const string ImagesPath = "images";
        private const string FtpHost = "ftp://ftp.chester.ru";
        public CSVParser CSVParser { get; private set; }
        public CartsViewModel vmCarts { get;   set; }
        string WorkDir;
        string RootPath { get; set; }
        public string LocalImagePath { get; set; }
        string _serialnumber { get; set; }
        public AnalizeSpare(string serialnumber)
        {
            vmCarts = new CartsViewModel();
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            RootPath = "/";
            _serialnumber = serialnumber;
            // ReadCSV();

        }


        public async Task ReadCSV(IProgressDialog progressDialog)
        {
            var filename = GetFileNameFromSerialNo(_serialnumber);
            string path = await DownLoad(CSVPath, $"{filename}.csv");
            string emailpath = await DownLoad(EmailsPath, $"brandemails.csv");
            LocalImagePath = await DownLoad(ImagesPath, $"{filename}.png");
            CSVParser = new CSVParser(path, emailpath);
            await DownLoadThumps(ThumpPath, CSVParser.Parts);
        }
        public ImageSource GetThump(string partnumber)
        {
            string path = Path.Combine(WorkDir, ThumpPath, $"{partnumber}.jpg");
            if (File.Exists(path))
                return ImageSource.FromFile(path);
            return null;
        }
        public Part[] CheckContain(int x, int y)
        {
            return CSVParser.CheckContainInRect(x, y);
        }




        public  string GetEmailTo()
        {
            if (CSVParser.Emails==null || CSVParser.Emails.Count == 0)
            {
                App.Dialogs.Alert("not find send to emails!");
                return "";
            }
            string bb = GetFileNameFromSerialNo(_serialnumber);
            bb = bb.Substring(bb.Length - 2);
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
            stringBuilder.AppendLine(string.Format(Models.Settings.Current.Resources["SharePartTitle"], _serialnumber));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Settings.Current.Resources["SharePartText2"]);// Do not edit this section");
            stringBuilder.AppendLine("-".PadRight(60,'-'));
            //
            foreach (Cart cart in carts)
            {
                stringBuilder.AppendLine($"{cart.Count},{cart.Part.PartNumber},{cart.Part.PartName}");
            }
            stringBuilder.AppendLine("-".PadRight(60, '-'));
            stringBuilder.AppendLine(Settings.Current.Resources["SharePartText3"]);//"Please supply your contact number below");
            return stringBuilder.ToString();
        }
        private string GetFileNameFromSerialNo(string serial)
        {
            return serial.Substring(0, 15);
           
        }
        #region FTP
        private async Task<string> DownLoad(string path, string filename)
        {
            var filenamepath = Path.Combine(WorkDir, path);
            if (!Directory.Exists(filenamepath))
            {
                Directory.CreateDirectory(filenamepath);
            }
            string resultfilepath= Path.Combine(filenamepath, filename);
            var remotefilename = $"/{path}/{filename}";
            FtpClient client = new FtpClient(GlobalConstants.FtpHost, GlobalConstants.FtpPort, new NetworkCredential("centri_clean", "AQHg8t)AQHg8t)"));
            try
            {

                 

                // begin connecting to the server
                await client.ConnectAsync();
                if (client.IsConnected)
                {


                    {

                        if (client.FileExists($"{remotefilename}"))
                        {
                            
                            bool dowloaded = false;
                            try
                            {
                                //using (var cancelSrc = new CancellationTokenSource())
                                //{
                                //    using (App.Dialogs.Loading(Settings.Current.Resources["DownloadWaitText"], cancelSrc.Cancel, Settings.Current.Resources["CancelText"]))
                                //    {
                                        string tmpFileName = Path.Combine(filenamepath, filename + DateTime.Now.Second.ToString());
                                        
                                        dowloaded = await client.DownloadFileAsync(tmpFileName, $"{remotefilename}", true);
                                        
                                        File.Copy(tmpFileName, resultfilepath, true);
                                        try
                                        {
                                            File.Delete(tmpFileName);
                                        }
                                        catch (Exception er)
                                        {

                                        }
                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                                
                                App.Dialogs.Toast($"{filename}: {ex.ToString()}");
                                //await App.Dialogs.AlertAsync(ex.ToString());

                            }
                            //if (dowloaded || File.Exists(filename))
                            //{
                            //    WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
                            //    await Navigation.PushAsync(webViewPageCS);
                            //}

                        }
                        else
                        {
                            
                            App.Dialogs.Toast($"{filename} {Settings.Current.Resources["ManualNotFoundText"]}");
                           // await App.Dialogs.AlertAsync(Settings.Current.Resources["ManualNotFoundText"]);
                        }



                    };
                }
                else
                {
                    
                    App.Dialogs.Toast(Settings.Current.Resources["NoInternetConOrErrorText"]);
                    //await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
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
            return resultfilepath;
        }
        private async Task<string> DownLoadThumps(string path, List<Part> partsnumbers)
        {
            var filenamepath = Path.Combine(WorkDir, path);
            if (!Directory.Exists(filenamepath))
            {
                Directory.CreateDirectory(filenamepath);
            }


            FtpClient client = new FtpClient(GlobalConstants.FtpHost, GlobalConstants.FtpPort, new NetworkCredential("centri_clean", "AQHg8t)AQHg8t)"));
            try
            {

            

                // begin connecting to the server
                await client.ConnectAsync();
                if (client.IsConnected)
                {

                    foreach (Part part in partsnumbers)
                    {
                        var filename = $"{part.PartNumber}.jpg";
                        var remotefilename = $"/{path}/{filename}";
                        if (client.FileExists($"{remotefilename}"))
                        {
                            //if (File.Exists(filename))
                            //{
                            //    File.Delete(filename);
                            //}
                            bool dowloaded = false;
                            try
                            {
                                //using (var cancelSrc = new CancellationTokenSource())
                                //{
                                //    using (App.Dialogs.Loading(Settings.Current.Resources["DownloadWaitText"], cancelSrc.Cancel, Settings.Current.Resources["CancelText"]))
                                //    {
                                        string tmpFileName = Path.Combine(filenamepath, filename + DateTime.Now.Second.ToString());
                                        dowloaded = await client.DownloadFileAsync(tmpFileName, $"{remotefilename}", true);
                                        filenamepath = Path.Combine(filenamepath, filename);
                                        File.Copy(tmpFileName, filenamepath, true);
                                        try
                                        {
                                            File.Delete(tmpFileName);
                                        }
                                        catch (Exception er)
                                        {
                                            continue;
                                        }
                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                              //  progressDialog.Hide();
                              //  App.Dialogs.HideLoading();
                                //await App.Dialogs.AlertAsync(ex.ToString());
                                App.Dialogs.Toast(ex.Message);
                                
                                continue;
                            }
                            //if (dowloaded || File.Exists(filename))
                            //{
                            //    WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
                            //    await Navigation.PushAsync(webViewPageCS);
                            //}

                        }
                        else
                        {
                            App.Dialogs.Toast($"{filename} {Settings.Current.Resources["ManualNotFoundText"]}");
                            //App.Dialogs.Toast(Settings.Current.Resources["ManualNotFoundText"]);
                            //Debug.WriteLine(Settings.Current.Resources["ManualNotFoundText"]);
                            //await App.Dialogs.AlertAsync(Settings.Current.Resources["ManualNotFoundText"]);
                        }



                    };
                }
                else
                {
                    
                    App.Dialogs.Toast(Settings.Current.Resources["NoInternetConOrErrorText"]);
                    //await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
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
