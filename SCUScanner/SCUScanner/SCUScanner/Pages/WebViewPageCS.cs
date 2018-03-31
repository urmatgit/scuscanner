using SCUScanner.Pages.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SCUScanner.Pages
{
	public class WebViewPageCS : ContentPage
	{
		public WebViewPageCS (string pdfFile)
		{
			Content = new StackLayout {
				Children = {
                    new CustomWebView {
                    Uri = pdfFile,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                } }
			};
		}
	}
}