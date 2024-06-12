using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Square.Models;
using BeeThere_OrderReport.Classes;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Windows.Storage;
using Windows.Storage.Pickers.Provider;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BeeThere_OrderReport
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintOptionsPage : Page
    {
        Stream filestream = null;
        string filename = null;

        public PrintOptionsPage()
        {
            this.InitializeComponent();
        }

        public async Task<List<Order>> GetOrders()
        {
            Classes.SquareAPIs.ISquareAPI sqAPI = new Classes.SquareAPIs.Sandbox();
            Classes.SquareAPIs.OrderCollector collector = new(sqAPI.GetClient());
            List<Order> orders = await collector.GetOrders(XamlRoot, sqAPI.GetLocationIDs());
            return orders;
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (filestream == null)
            {
                await ContentDialogMaker.Run(xamlRoot: XamlRoot, message: "You haven't selected a file location.\n A file location is required to save the file.", title: "Wait a second!", button_text: "OK");
                return;
            }

            List<Order> orders = await GetOrders();
            if (orders.Count == 0) { return; }

            Classes.SquareAPIs.ISquareAPI sqAPI = new Classes.SquareAPIs.Sandbox();
            
            await OrderReportGenerator.Generate(XamlRoot, sqAPI.GetClient(), orders, filestream);
            
            await filestream.FlushAsync();
            filestream.Close();

            await ContentDialogMaker.Run(xamlRoot: XamlRoot, message: "The report has been saved to " + filename + ".", title: "File saved!", button_text: "OK");
        }

        private async void FilePickButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".pdf";
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.SuggestedFileName = "Orders_" + DateTime.Now.ToString("yy_MM_dd_hh_mm_ss");
            savePicker.CommitButtonText = "OK";
            savePicker.FileTypeChoices.Add("PDF", new List<string>() { ".pdf" });

            StorageFile file = null;

            nint windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
            InitializeWithWindow.Initialize(savePicker, windowHandle);

            file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                filename = file.Name;
                filestream = await file.OpenStreamForWriteAsync();
                return;
            }
            else
            {
                await ContentDialogMaker.Run(xamlRoot: XamlRoot, message: "File loaded incorrectly.\nPlease choose another file.", title: "File Error", button_text: "OK");
            }
        }

    }
}
