using Acr.UserDialogs;
using FluentFTP;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
   public static class Utils
    {
        public static string GetFileNameFromSerialNo(string serial,string lang)
        {

            string result = "";
            //int index_ = serial.LastIndexOf('-');
            //if (index_ <= 0)
            //    index_ = serial.Length;
            //result = serial.Substring(0, index_).ToUpper();
            result = serial.Substring(0, 15);
            result = $"{result}({lang}).pdf";
            return result;
        }
        public static async Task  DownloadManual<TParam>(string serial,string kod, IProgressDialog progressDialog, Func<string, Task> navigate)
        {
             var  WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            WorkDir = Path.Combine(WorkDir, "manuals");
            if (!Directory.Exists(WorkDir))
            {
                Directory.CreateDirectory(WorkDir);
            }
            string filename = Utils.GetFileNameFromSerialNo(serial, kod);

            var filenamelocal = Path.Combine(WorkDir,  filename);
            FtpClient client = new FtpClient(GlobalConstants.FtpHost,GlobalConstants.FtpPort, new NetworkCredential("centri_clean", "AQHg8t)AQHg8t)"));
            try
            {

             
                // begin connecting to the server
                await client.ConnectAsync();
                if (client.IsConnected)
                {


                    {

                        if (client.FileExists($"/manuals/{filename}"))
                        {
                            
                            bool dowloaded = false;
                            try
                            {
                                using (var cancelSrc = new CancellationTokenSource())
                                {
                                    using (App.Dialogs.Loading(Settings.Current.Resources["DownloadText"], cancelSrc.Cancel, Settings.Current.Resources["CancelText"]))
                                    {
                                        string tmpFileName = filenamelocal + DateTime.Now.Second.ToString();
                                        dowloaded = await client.DownloadFileAsync(tmpFileName, $"/manuals/{filename}", true);
                                        File.Copy(tmpFileName, filenamelocal, true);
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
                                progressDialog.Hide();
                                App.Dialogs.HideLoading();
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
                            progressDialog.Hide();
                            App.Dialogs.HideLoading();
                            await App.Dialogs.AlertAsync(Settings.Current.Resources["ManualNotFoundText"]);
                        }



                    };
                }
                else
                {
                    progressDialog.Hide();
                    App.Dialogs.HideLoading();
                    await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);
                }
            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                App.Dialogs.HideLoading();
                await App.Dialogs.AlertAsync(Settings.Current.Resources["NoInternetConOrErrorText"]);

            }
            finally
            {
                await client?.DisconnectAsync();
                if (File.Exists(filenamelocal))
                {
                   await navigate(filenamelocal);
                    //WebViewPageCS webViewPageCS = new WebViewPageCS(filenamelocal);
                    //await Navigation.PushAsync(webViewPageCS);
                }
            }
        }

       
    }
}
