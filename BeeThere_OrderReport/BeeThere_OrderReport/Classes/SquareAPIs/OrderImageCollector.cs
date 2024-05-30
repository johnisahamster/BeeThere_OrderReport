using QuestPDF.Infrastructure;
using Square;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeeThere_OrderReport.Classes.SquareAPIs
{
    internal class OrderImageCollector
    {
        Dictionary<string, CatalogImage> images;
        bool ready = false;

        public OrderImageCollector(ISquareClient client)
        {
            this.client = client;
            
            ListCatalogResponse response = client.CatalogApi.ListCatalog(null, "IMAGE");
            foreach (var item in response.Objects)
            {
                images.Add(item.Id, item.ImageData);
            }
            ready = true;
        }

        private readonly ISquareClient client;
        public ISquareClient Client { get { return client; } }

        public async Task<List<byte[]>> GetImages(Order order)
        {
            List<byte[]> output = new();
            while (!ready) { }

            foreach (var item in order.LineItems)
            {   
                output.Add(await DownloadImage(images[item.CatalogObjectId].Url));
            }

            return output;
        }

        public async Task<byte[]> DownloadImage(string imageUrl)
        {
            HttpClient client = new HttpClient();
            Stream stream = await client.GetStreamAsync(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                ImageConverter converter = new ImageConverter();
                var output = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                stream.Flush();
                stream.Close();
                client.Dispose();

                return output;
            }
            else
            {
                stream.Flush();
                stream.Close();
                client.Dispose();

                return null;
            }
        }
    }
}
