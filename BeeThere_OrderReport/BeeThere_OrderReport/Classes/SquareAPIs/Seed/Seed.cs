using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Square.Models;
using Windows.Security.Cryptography.Core;
using WinRT;

namespace BeeThere_OrderReport.Classes.SquareAPIs.Seed
{
    internal class Seed
    {
        private readonly Square.ISquareClient client;
        private string hash;

        public Seed()
        {
            ISquareAPI api = new Sandbox();
            client = api.GetClient();

            hash = "4429380778C94E47427C1753BAF91E0D8AF78985AA9F3868CF3FC07456F7BAFA"; 
        }

        public void Run()
        {
            GenerateCustomers(); //1
            //Generate Catalog Items
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
                        .Country("Canada")
                        .PostalCode(customers[i].PostalCode)
                        .FirstName(customers[i].FirstName)
                        .LastName(customers[i].LastName)
                        .Build()
                    )
                    .Birthday(customers[i].DateOfBirth.ToString("MM-DD"))
                    .EmailAddress(customers[i].EmailAddress)
                    .PhoneNumber(customers[i].PhoneNumber)
                    .IdempotencyKey(hash + 1.ToString() + i.ToString())
                    .Build();
                client.CustomersApi.CreateCustomer(request);
            }

            return customers;
        }

        public IList<CatalogItem> GenerateCatalogItems(int n = 5)
        {
            List<CatalogItem> items = new();

            for (int i = 0; i < n; i++)
            {
                items.Add(new CatalogItem());

                UpsertCatalogObjectRequest request = new UpsertCatalogObjectRequest.Builder(
                        hash + 2.ToString() + i.ToString(),
                        new CatalogObject.Builder("ITEM", "#" + items[i].Name)
                            .ImageData(new CatalogImage(null, items[i].ImageURL))
                            .ItemData(
                                new Square.Models.CatalogItem.Builder()
                                .Name(items[i].Name)
                                .Build()
                            ).Build()
                ).Build();
            }

            return items;
        }


    }
}
