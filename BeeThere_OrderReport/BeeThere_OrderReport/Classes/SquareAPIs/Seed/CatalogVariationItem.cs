using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeThere_OrderReport.Classes.SquareAPIs.Seed
{
    enum PricingType 
    {
        Fixed,
        Variable
    }

    internal class CatalogVariationItem
    {
        public string ItemID { get; set; }
        public string Name { get; set; }
        public PricingType PricingType { get; set; }
        public int PriceValue { get; set; }
        public string PriceCurrency { get; set; }
        public string SKU { get; set; }
    
        public CatalogVariationItem() {
            Name = Faker.Name.Middle();
            PricingType = Faker.Enum.Random<PricingType>();
            PriceValue = Faker.RandomNumber.Next(0, 10000);
            PriceCurrency = "CAD";
            SKU = this.GetHashCode().ToString() + "FAKE";
        }
    }
}
