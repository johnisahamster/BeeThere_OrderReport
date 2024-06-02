using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using static System.Net.WebRequestMethods;

namespace BeeThere_OrderReport.Classes.SquareAPIs.Seed
{
    internal class CatalogItem
    {
        public int ID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public List<CatalogVariationItem> ItemVariations {  get; set; }

        public CatalogItem()
        {
            ItemVariations = new List<CatalogVariationItem>();
            ImageURL = "https://handletheheat.com/wp-content/uploads/2023/06/peanut-butter-chocolate-chip-cookies-SQUARE.jpg";
            Name = Faker.Internet.UserName();
            for (int i = 0; i < Faker.RandomNumber.Next(1,3);  i++)
            {
                ItemVariations.Add(new CatalogVariationItem());
            }
            ID = this.GetHashCode();
        }


        public override string ToString()
        {
            string output = Name;
            output += (ItemVariations.Count > 0) ? ": " : "";
            foreach (var variation in ItemVariations) { output += variation.Name + ", "; }
            return output;
        }
    }
}