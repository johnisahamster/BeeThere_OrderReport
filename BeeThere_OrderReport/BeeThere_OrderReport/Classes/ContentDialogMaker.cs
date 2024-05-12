using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Square.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace BeeThere_OrderReport.Classes
{
    internal class ContentDialogMaker
    {
        public static async Task<ContentDialogResult> Run(XamlRoot xamlRoot, string message)
        {
            ContentDialog dialog = new()
            {
                Content = message,
                XamlRoot = xamlRoot,
                CloseButtonText = "Ok"
            };

            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> Run(XamlRoot xamlRoot, string message, string title)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = message,
                XamlRoot = xamlRoot,
                CloseButtonText = "Ok"
            };

            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> Run(XamlRoot xamlRoot, string message, string title, string button_text)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = message,
                XamlRoot = xamlRoot,
                CloseButtonText = button_text
            };

            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> APIError(XamlRoot xamlRoot, ApiException e, string message = "", string title = "API Exception")
        {
            string output_msg = "";

            if (message != "")
            {
                output_msg = message + e.Message + "\n";
            }
            else
            {
                output_msg = "API Error: " + e.Message + "\n";
            }
            foreach (var item in e.Errors)
            {
                message += item.Code + " - " + item.Detail + "\n";
            }

            ContentDialog dialog = new()
            {
                Title = title,
                Content = message,
                XamlRoot = xamlRoot,
                CloseButtonText = "Ok"
            };

            return await dialog.ShowAsync();
        }
    }
}
