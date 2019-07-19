using Acr.UserDialogs;
using FluentFTP;
using Microsoft.AppCenter.Crashes;
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
        private const string EmailsFile = "brandsalesemail.csv";
        private const string ThumpPath = "thumbnails";
        private const string ImagesPath = "images";
        private const string FtpHost = "ftp://ftp.chester.ru";
        public CSVParser CSVParser { get; private set; }
        public CartsViewModel vmCarts { get;   set; }
        string WorkDir;
        string RootPath { get; set; }
        public string LocalImagePath { get; set; }
        string _serialnumber { get; set; }
        public bool ErrorConnect { get; set; } = false;
        public AnalizeSpare(string serialnumber)
        {
            vmCarts = new CartsViewModel();
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            RootPath = "/";
            _serialnumber = serialnumber;
            // ReadCSV();

        }

        public void ConvertToNewScale(float dexX,float dexY)
        {
            foreach (Part part in CSVParser.Parts)
            {
                part.ReSize (dexX, dexY);
                
            }
        }
        public async Task ReadCSV(IProgressDialog progressDialog,bool uselocal=false)
        {
            ErrorConnect = uselocal;
            FtpClient client=null;
            if (!uselocal)
            {
                try
                {
                    client = new FtpClient(GlobalConstants.FtpHost, GlobalConstants.FtpPort, new NetworkCredential("centri_clean", "AQHg8t)AQHg8t)"));

                }
                catch (Exception er)
                {
                     Crashes.TrackError(er, new Dictionary<string,string>{
                    { "Class", "AnalizeSpare" },
                    { "functions", "ReadCSV" },
                    { "Issue", $"FtpClient({GlobalConstants.FtpHost})"}
                    });
                    ErrorConnect = true;
                    await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);

                }
            }
            var filename = GetFileNameFromSerialNo(_serialnumber);
            string path = await DownLoad(CSVPath, $"{filename}.csv",client);
            
            string emailpath = await DownLoad(EmailsPath, EmailsFile, client);
            //if (ErrorConnect)
            //    LocalImagePath = "SCUScanner.img.MP60002_0100100.png";
            //else 
                LocalImagePath = await DownLoad(ImagesPath, $"{filename}.png", client);
            //if (File.Exists(App.analizeSpare.LocalImagePath))
            //{
            //    App.Dialogs.Toast($"Loaded {App.analizeSpare.LocalImagePath}");
            //}else
            //    App.Dialogs.Toast($"File not found {App.analizeSpare.LocalImagePath}");
            CSVParser = new CSVParser(path, emailpath);
            await DownLoadThumps(ThumpPath, CSVParser.Parts,client);
            if (!uselocal)
               await client?.DisconnectAsync();
            
        }
        public ImageSource GetThump(string partnumber)
        {
            string path = Path.Combine(WorkDir, ThumpPath, $"{partnumber}.jpg");
            if (File.Exists(path))
                return ImageSource.FromFile(path);
            return null;
        }
        public Part[] CheckContain(float x, float y)
        {
            return CSVParser.CheckContainInRect(x, y);
        }
        public Part[] CheckContain(float x, float y, float dx, float dy, float tx, float ty)
        {
            return CSVParser.CheckContainInRect(x, y,dx,dy,tx,ty);
            //    return CSVParser.CheckContainInRect(x, y);
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
            if (serial.Length < 15) return serial;
            return serial.Substring(0, 15);
           
        }
        #region FTP
        private async Task<string> DownLoad(string path, string filename, FtpClient client)
        {
            var filenamepath = Path.Combine(WorkDir, path);
            if (!Directory.Exists(filenamepath))
            {
                Directory.CreateDirectory(filenamepath);
            }
            string resultfilepath= Path.Combine(filenamepath, filename);
            if (ErrorConnect || client==null) return resultfilepath;
            var remotefilename = $"/{path}/{filename}";
            
            try
            {



                // begin connecting to the server
                if (!client.IsConnected)
                    await client.ConnectAsync();
                if (client.IsConnected)
                {


                    {
                    
                        if ( client.FileExists($"{remotefilename}"))
                        {
                            FileInfo localfile = new FileInfo(resultfilepath);

                            //Debug.WriteLine($"client.GetFileSize(remotefilename)-{client.GetFileSize(remotefilename)}");
                            
                            
                            //Debug.WriteLine($"client.GetModifiedTime-{client.GetModifiedTime(remotefilename)}");

                            //Debug.WriteLine($"localfile.Length-{localfile.Length}");
                            //Debug.WriteLine($"localfile.LastWriteTime-{localfile.LastWriteTime}");

                            
                            if (!File.Exists(resultfilepath) || client.GetFileSize(remotefilename) != localfile.Length )
                            {
                                
                                bool dowloaded = false;
                                try
                                {
                                    //using (var cancelSrc = new CancellationTokenSource())
                                    //{
                                    //    using (App.Dialogs.Loading(Settings.Current.Resources["DownloadWaitText"], cancelSrc.Cancel, Settings.Current.Resources["CancelText"]))
                                    //    {
                                    string tmpFileName = Path.Combine(filenamepath, filename + DateTime.Now.Second.ToString());

                                    dowloaded = await client.DownloadFileAsync(tmpFileName, $"{remotefilename}", FtpLocalExists.Overwrite);

                                    File.Copy(tmpFileName, resultfilepath, true);
                                    try
                                    {
                                        File.Delete(tmpFileName);
                                    }
                                    catch (Exception er)
                                    {
                                    Crashes.TrackError(er, new Dictionary<string,string>{
                                        { "Class", "AnalizeSpare" },
                                        { "functions", "DownLoad" },
                                        { "Issue", $"Delete({tmpFileName})"}
                                        });
                                    }
                                    App.Dialogs.Toast($"{Settings.Current.Resources["DownloadedText"]}- {filename}");
                                    //    }
                                    //}
                                }
                                catch (Exception ex)
                                {

                                Crashes.TrackError(ex, new Dictionary<string,string>{
                                        { "Class", "AnalizeSpare" },
                                        { "functions", "DownLoad" },
                                        { "Issue", $"DownloadFileAsync({remotefilename})"}
                                        });
                                    App.Dialogs.Toast($"{filename}: {ex.ToString()}");
                                    //await App.Dialogs.AlertAsync(ex.ToString());

                                }
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
                    ErrorConnect = true;
                    App.Dialogs.Toast(Settings.Current.Resources["NoInternetConOrErrorText"]);
                    //await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string,string>{
                                        { "Class", "AnalizeSpare" },
                                        { "functions", "DownLoad" },
                                        { "Issue", Settings.Current.Resources["NoInternetConOrErrorText"]}
                                        });
                ErrorConnect = true;
                await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
                filenamepath = "";
            }
            //finally
            //{
            //    await client?.DisconnectAsync();
            //}
            return resultfilepath;
        }
        private async Task<string> DownLoadThumps(string path, List<Part> partsnumbers, FtpClient client)
        {
            var filenamepath = Path.Combine(WorkDir, path);
            if (!Directory.Exists(filenamepath))
            {
                Directory.CreateDirectory(filenamepath);
            }

            if (ErrorConnect || client==null)
            {

                return filenamepath;
            }

            try
            {

            
                if (!client.IsConnected)
                    await client.ConnectAsync();
                if (client.IsConnected)
                {

                    foreach (Part part in partsnumbers)
                    {
                        var filename = $"{part.PartNumber}.jpg";
                        var remotefilename = $"/{path}/{filename}";
                        if (client.FileExists($"{remotefilename}"))
                        {
                            var filenlocal = Path.Combine(filenamepath, filename);
                            FileInfo localfile = new FileInfo(filenlocal);
                            if (!File.Exists(filenlocal) || client.GetFileSize(remotefilename) != localfile.Length )
                            {
                                bool dowloaded = false;
                                try
                                {
                                    //using (var cancelSrc = new CancellationTokenSource())
                                    //{
                                    //    using (App.Dialogs.Loading(Settings.Current.Resources["DownloadWaitText"], cancelSrc.Cancel, Settings.Current.Resources["CancelText"]))
                                    //    {
                                    string tmpFileName = Path.Combine(filenamepath, filename + DateTime.Now.Second.ToString());
                                    dowloaded = await client.DownloadFileAsync(tmpFileName, $"{remotefilename}", FtpLocalExists.Overwrite);
                                 
                                    //if (File.Exists(filenlocal))
                                    //    File.Delete(filenlocal);
                                    File.Copy(tmpFileName, filenlocal, true);
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
           
            return filenamepath;
        }
        #endregion
    }
}
