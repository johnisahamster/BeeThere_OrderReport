using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeThere_OrderReport.Classes.SquareAPIs;
using Microsoft.UI.Xaml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Square;
using Square.Models;

namespace BeeThere_OrderReport.Classes
{
    internal class OrderReportGenerator
    {

        public static async Task Generate(XamlRoot xamlRoot, ISquareClient client, List<Order> orders, Stream stream)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            List<IDocument> doc = new List<IDocument>();
            OrderImageCollector collector = new OrderImageCollector(xamlRoot, client);
            Dictionary<string, byte[]> images_from_object_id = new();//await collector.GetImagesFromOrders(orders);

            foreach (Order order in orders)
            {
                doc.Add(MakeDoc(order, images_from_object_id));
            }

            Document.Merge(doc).GeneratePdf(stream);
        }

        private static IDocument MakeDoc(Order order, Dictionary<string, byte[]> images)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    //page data
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    //page content
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text("Order Details - " + order.Id);
                            x.Item().Table(x =>
                            {
                                x.ColumnsDefinition(columns => {
                                    columns.RelativeColumn(); //image
                                    columns.RelativeColumn(); //item
                                    columns.RelativeColumn(); //price
                                    columns.RelativeColumn(); //qty
                                    columns.RelativeColumn(); //total
                                });

                                for (int i = 0; i < order.LineItems.Count; i++)
                                {
                                    //x.Cell().Element(Block).Image(images[order.LineItems[i].CatalogObjectId]); //image
                                    x.Cell().Element(Block).Image(Placeholders.Image(200,200));
                                    x.Cell().Element(Block).Text(order.LineItems[i].Name); //item
                                    x.Cell().Element(Block).Text(order.LineItems[i].BasePriceMoney.ToString()); //price
                                    x.Cell().Element(Block).Text(order.LineItems[i].Quantity); //qty
                                    x.Cell().Element(Block).Text((order.LineItems[i].BasePriceMoney.Amount * long.Parse(order.LineItems[i].Quantity)).ToString()); //total
                                }
                            });

                            //notes //subtotals
                            //total
                            //fulfillment details //billing details
                            //time of order
                        });

                    //footer
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span("/");
                            x.TotalPages();
                        });
                });
            });
        }

        //DEFAULT TABLE SETTINGS
        static IContainer Block(IContainer container)
        {
            return container
                .Border(1)
                .Background(Colors.Grey.Lighten3)
                .ShowOnce()
                .MinWidth(50)
                .MinHeight(50)
                .AlignCenter()
                .AlignMiddle();
        }
    }
}