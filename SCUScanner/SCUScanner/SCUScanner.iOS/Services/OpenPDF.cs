using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CentriClean.Services;
using Foundation;
using QuickLook;
using SCUScanner.Helpers;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(OpenPDF))]
namespace CentriClean.Services
{
    

    public class OpenPDF : IOpenPDF
    {
         

        public void OpenPdf(string filePath, string needPermission = "", string notPermisson = "", string noApplication = "")
        {
            FileInfo fi = new FileInfo(filePath);
            UINavigationController controller = FindNavigationController();
            if (controller != null)
            {
                UINavigationBar.Appearance.TintColor = UIColor.Black;
                UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.Black };
                QLPreviewController previewController = new QLPreviewController();
                previewController.DataSource = new PDFPreviewControllerDataSource(fi.FullName, fi.Name);
                controller.PresentViewController(previewController, true, ()=>
                {
                    UINavigationBar.Appearance.TintColor = UIColor.White;
                    UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
                });
                
            }
        }

        private UINavigationController FindNavigationController()
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController.NavigationController != null)
                    return window.RootViewController.NavigationController;
                else
                {
                    UINavigationController val = CheckSubs(window.RootViewController.ChildViewControllers);
                    if (val != null)
                        return val;
                }
            }

            return null;
        }

        private UINavigationController CheckSubs(UIViewController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.NavigationController != null)
                    return controller.NavigationController;
                else
                {
                    UINavigationController val = CheckSubs(controller.ChildViewControllers);
                    if (val != null)
                        return val;
                }
            }
            return null;
        }




    }
    
    public class PDFItem : QLPreviewItem
    {
        string title;
        string uri;

        public PDFItem(string title, string uri)
        {
            this.title = title;
            this.uri = uri;
        }

        public override string ItemTitle
        {
            get { return title; }
        }

        public override NSUrl ItemUrl
        {
            get { return NSUrl.FromFilename(uri); }
        }
    }
    public class PDFPreviewControllerDataSource : QLPreviewControllerDataSource
    {
        string url = "";
        string filename = "";

        public PDFPreviewControllerDataSource(string url, string filename)
        {
            this.url = url;
            this.filename = filename;
        }
        
        //public override QLPreviewItem GetPreviewItem(QLPreviewController controller, int index)
        //{
        //    return new PDFItem(filename, url);
        //}

        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
        {
            return new PDFItem(filename, url);
        }
        
        public override nint PreviewItemCount(QLPreviewController controller)
        {
            return 1;
        }
    }
}