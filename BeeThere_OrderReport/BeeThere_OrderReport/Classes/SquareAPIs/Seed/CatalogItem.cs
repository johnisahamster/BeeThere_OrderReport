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
        public List<CatalogItemVariation> ItemVariations {  get; set; }

        public CatalogItem()
        {
            ImageURL = "https://handletheheat.com/wp-content/uploads/2023/06/peanut-butter-chocolate-chip-cookies-SQUARE.jpg";
            Name = Faker.Lorem.GetFirstWord();
            for (int i = 0; i < Faker.RandomNumber.Next(1,3);  i++)
            {
                ItemVariations.Add(new CatalogItemVariation());
            }
        }
    }
}