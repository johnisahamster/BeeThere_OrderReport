using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Square.Models;
using Windows.Security.Cryptography.Core;
using WinRT;
using Square.Http.Client;
using System.IO;
using System.Net.Http;

namespace BeeThere_OrderReport.Classes.SquareAPIs.Seed
{
    internal class Seed
    {
        private readonly Square.ISquareClient client;
        private string hash;
        private string locationID;

        public Seed()
        {
            ISquareAPI api = new Sandbox();
            client = api.GetClient();

            hash = "4429380778C94E47427C1753BAF91E0D8AF78985AA9F3868CF3FC07456F7BAFA";
            locationID = client.LocationsApi.ListLocations().Locations[0].Id;
        }

        public async void Run()
        {
            //IList<Customer> customers = GenerateCustomers(); //1
            IList<CatalogItem> items = await GenerateCatalogItems(); //2,3
            //Generate Orders

            //System.Diagnostics.Debug.WriteLine("customers:");
            //foreach (Customer customer in customers) { System.Diagnostics.Debug.WriteLine(customer.ToString()); }
            System.Diagnostics.Debug.WriteLine("items:");
            foreach (CatalogItem item in items) { System.Diagnostics.Debug.WriteLine(item.ToString()); }
        }

        //==================================================

        public IList<Customer> GenerateCustomers(int n = 5)
        {
            List<Customer> customers = new();

            for (int i = 0; i < n; i++)
            {
                customers.Add(new Customer());

                CreateCustomerRequest request = new CreateCustomerRequest.Builder()
                    .Address(new Square.Models.Address.Builder()
                        .AddressLine1(customers[i].Address)
                        .Locality(customers[i].City)
                        .Country("CA")
                        .PostalCode(customers[i].PostalCode)
                        .FirstName(customers[i].FirstName)
                        .LastName(customers[i].LastName)
                        .Build()
                    )
                    .Birthday(customers[i].DateOfBirth.ToString("MM-dd"))
                    .EmailAddress(customers[i].EmailAddress)
                    //.PhoneNumber(customers[i].PhoneNumber)
                    .IdempotencyKey(hash + 1.ToString() + i.ToString())
                    .Build();
                client.CustomersApi.CreateCustomer(request);
            }

            return customers;
        }

        public async Task<IList<CatalogItem>> GenerateCatalogItems(int n = 5)
        {
            List<CatalogItem> items = new();

            var imageData = new CatalogImage.Builder()
              .Caption("A picture of a couple cookies")
              .Build();
            var image = new CatalogObject.Builder(type: "IMAGE", id: "#COOKIEIMG")
              .ImageData(imageData)
              .Build();
            var imagerequest = new CreateCatalogImageRequest.Builder(idempotencyKey: hash + "img", image: image)
              .ObjectId("ND6EA5AAJEO5WL3JNNIAQA32")
              .Build();

            HttpClient httpclient = new HttpClient();
            Stream stream = await httpclient.GetStreamAsync("https://handletheheat.com/wp-content/uploads/2023/06/peanut-butter-chocolate-chip-cookies-SQUARE.jpg");
            var imageFile = new FileStreamInfo(stream);

            await client.CatalogApi.CreateCatalogImageAsync(imagerequest);

            for (int i = 0; i < n; i++)
            {
                items.Add(new CatalogItem());

                var request = new UpsertCatalogObjectRequest.Builder(
                    hash + 2.ToString() + i.ToString(),
                    new CatalogObject.Builder("ITEM", "#" + items[i].Name)
                        .ItemData(
                            new Square.Models.CatalogItem.Builder()
                            .Name(items[i].Name)
                            .ImageIds(new List<string> { "#COOKIEIMG" })
                            .Build()
                        ).Build()
                ).Build();

                client.CatalogApi.UpsertCatalogObject(request);

            }

            return items;
        }

        public async void GenerateOrders(int n = 5) //TODO: move this function to Order.cs class
        {
            BatchRetrieveInventoryCountsRequest invreq = new BatchRetrieveInventoryCountsRequest.Builder().Build();
            BatchRetrieveInventoryCountsResponse inv = client.InventoryApi.BatchRetrieveInventoryCounts(invreq);
            ListCatalogResponse objs = new ListCatalogResponse.Builder().Build();

            List<Order> orders = new List<Order>();
            List<InventoryChange> changes = new List<InventoryChange>();

            int count = inv.Counts.Count; //
            int numitems; //number of items in order
            int k = 0, quantity = 0;

            for (int i = 0; i < n; i++) //orders
            {
                k = Faker.RandomNumber.Next(0, count); //pick random item
                quantity = int.Parse(inv.Counts[k].Quantity);

                numitems = Faker.RandomNumber.Next(1, 8); //random number of items in order
                for (int j = 0; j < numitems; j++) //orderitems
                {
                    while (quantity == 0) {
                        k = Faker.RandomNumber.Next(0, count); //pick random item
                        quantity = int.Parse(inv.Counts[k].Quantity);
                    }

                    orders.Add(new Order.Builder(locationID).Build()); //add item to order

                    //decrease inventory by 1
                    changes.Add(new InventoryChange.Builder().Adjustment(new InventoryAdjustment.Builder().CatalogObjectId(objs.Objects[k].Id).FromState("IN_STOCK").ToState("SOLD").Quantity(1.ToString()).Build()).Build());
                }
            }
            
            BatchChangeInventoryRequest inventoryChangesRequest = new BatchChangeInventoryRequest.Builder(
                    hash + 4.ToString() + count.ToString()
                ).Changes(changes).Build(); //reduce inventories by 1
            

            throw new NotImplementedException();
        }
    }
}
