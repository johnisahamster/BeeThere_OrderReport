using BeeThere_OrderReport.Classes.SquareAPIs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Square.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BeeThere_OrderReport
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public async void GetOrders()
        {
            Classes.SquareAPIs.ISquareAPI sqAPI = new Classes.SquareAPIs.Sandbox();
            Classes.SquareAPIs.OrderCollector collector = new(sqAPI.GetClient());
            Queue<Order> orders = await collector.GetOrders(spRoot.XamlRoot, sqAPI.GetLocationIDs());

            foreach (Order order in orders)
            {
                Console.WriteLine(order);
            }
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            GetOrders();
        }
    }

}
