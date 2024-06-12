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
        XamlRoot xamlRoot;
        bool ready = false;
        Dictionary<string, string> object_image_pairs;

        public OrderImageCollector(XamlRoot xamlRoot, ISquareClient client)
        {
            this.client = client;
            this.xamlRoot = xamlRoot;
            object_image_pairs = new Dictionary<string, string>();

            ready = true;
        }

        private readonly ISquareClient client;
        public ISquareClient Client { get { return client; } }

        public async Task<Dictionary<string, byte[]>> GetImagesFromOrders(List<Order> orders)
        {
            Dictionary<string,byte[]> imageid_imagepairs = await GetImagesFromIDs( 
                await GetImageIDs(GetObjectIDs(orders))
            );
            return ConnectObjectIDsToImages(imageid_imagepairs);
        }

        private static List<string> GetObjectIDs(List<Order> orders)
        {
            List<string> objids = new();

            //create list of unique object ids from line items
            foreach (var order in orders)
            {
                foreach (var item in order.LineItems)
                {
                    if (!objids.Contains(item.CatalogObjectId))
                    {
                        objids.Add(item.CatalogObjectId);
                    }
                }
            }

            return objids;
        }

        private async Task<List<string>> GetImageIDs(List<string> object_ids)
        {
            BatchRetrieveCatalogObjectsRequest objsreq = new BatchRetrieveCatalogObjectsRequest.Builder(object_ids).Build();
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

            List<string> image_ids = new();

            foreach (var obj in objectsResponse.Objects)
            {
                try
                {
                    var imageid = obj.ItemData.ImageIds[0];
                    object_image_pairs.Add(obj.Id, imageid);
                    if (!image_ids.Contains(imageid))
                    {
                        image_ids.Add(imageid);
                    }
                }
                catch (Exception e) 
                {
                    object_image_pairs.Add(obj.Id, null);
                }
            }

            return image_ids;
        }

        private async Task<Dictionary<string, byte[]>> GetImagesFromIDs(List<string> imageids)
        {
            BatchRetrieveCatalogObjectsRequest request = new BatchRetrieveCatalogObjectsRequest
                .Builder(imageids).Build();
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

            Dictionary<string, byte[]> images = new();

            foreach (var image in response.Objects)
            {
                images.Add(image.Id, await DownloadImage(image.ImageData.Url));
            }

            return images;
        }

        private Dictionary<string, byte[]> ConnectObjectIDsToImages(Dictionary<string, byte[]> images)
        {
            Dictionary<string, byte[]> images_from_obj_ids = new();
            foreach (var obj in object_image_pairs)
            {
                images_from_obj_ids.Add(obj.Key, images[obj.Value]);
            }
            return images_from_obj_ids;
        }

        private static async Task<byte[]> DownloadImage(string imageUrl)
        {
            HttpClient client = new();
            Stream stream = await client.GetStreamAsync(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                ImageConverter converter = new();
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
