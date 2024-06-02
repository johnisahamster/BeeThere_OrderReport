using Microsoft.UI.Xaml;
using QuestPDF.Infrastructure;
using Square;
using Square.Exceptions;
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
        Dictionary<string, string> images;
        Dictionary<string, string> item_image_pairs;
        XamlRoot xamlRoot;
        bool ready = false;

        public OrderImageCollector(XamlRoot xamlRoot, ISquareClient client)
        {
            this.client = client;
            this.xamlRoot = xamlRoot;
            images = new Dictionary<string, string>();

            ready = true;
        }

        private readonly ISquareClient client;
        public ISquareClient Client { get { return client; } }

        public async Task GetItemImagePairs(Order order)
        {
            List<string> objids = new();
            foreach (OrderLineItem item in order.LineItems)
            {
                objids.Add(item.CatalogObjectId);
            }
            BatchRetrieveCatalogObjectsRequest objsreq = new BatchRetrieveCatalogObjectsRequest.Builder(objids).Build();

            BatchRetrieveCatalogObjectsResponse objectsResponse = new();

            try
            {
                objectsResponse = await client.CatalogApi.BatchRetrieveCatalogObjectsAsync(objsreq);
            }
            catch (ApiException e)
            {
                string message = "ERROR! Something happened while trying to reach the Square API:\n";
                await ContentDialogMaker.APIError(xamlRoot, e, message, "Square API Exception");
                throw;
            }

            foreach (var obj in objectsResponse.Objects)
            {
                try
                {
                    var imageid = obj.ItemVariationData.ImageIds[0];
                    item_image_pairs.Add(obj.Id, imageid);
                    if (!images.ContainsKey(imageid))
                    {
                        images.Add(imageid, null);
                    }
                }
                catch 
                { 
                    item_image_pairs.Add(obj.Id, null); 
                }
            }
        }

        private async Task GetImageUrls()
        {
            BatchRetrieveCatalogObjectsRequest request = new BatchRetrieveCatalogObjectsRequest
                .Builder(images.Keys.ToList()).Build();

            BatchRetrieveCatalogObjectsResponse response = new();

            try
            {
                response = await client.CatalogApi.BatchRetrieveCatalogObjectsAsync(request);
            }
            catch (ApiException e)
            {
                string message = "ERROR! Something happened while trying to reach the Square API:\n";
                await ContentDialogMaker.APIError(xamlRoot, e, message, "Square API Exception");
                throw;
            }

            foreach(var image in response.Objects)
            {
                images[image.Id] = image.ImageData.Url;
            }
        }

        public async Task<List<byte[]>> GetImages(Order order)
        {
            List<byte[]> output = new();
            while (!ready) { }

            await GetItemImagePairs(order);

            foreach (var imageid in order.LineItems)
            {   
                output.Add(await DownloadImage(""));
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
