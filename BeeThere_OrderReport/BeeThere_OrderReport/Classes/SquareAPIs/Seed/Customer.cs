using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faker;

namespace BeeThere_OrderReport.Classes.SquareAPIs.Seed
{
    internal class Customer
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        //public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }

        public Customer()
        {
            FirstName = Faker.Name.First();
            LastName = Faker.Name.Last();
            Address = Faker.Address.StreetAddress();
            City = Faker.Address.City();
            //PhoneNumber = Faker.Phone.Number();
            PostalCode = PostalCodeGen();
            DateOfBirth = Faker.Identification.DateOfBirth();
            EmailAddress = Faker.Internet.Email();
        }

        private static string PostalCodeGen()
        {
            string output = "";

            output += Faker.Lorem.Words(1).ToList<string>()[0][0];
            output += Faker.RandomNumber.Next(0, 10).ToString();
            output += Faker.Lorem.Words(1).ToList<string>()[0][0];
            output += Faker.RandomNumber.Next(0, 10).ToString();
            output += Faker.Lorem.Words(1).ToList<string>()[0][0];
            output += Faker.RandomNumber.Next(0, 10).ToString();

            return output;
        }

        public override string ToString()
        {
            return Id + ": " + FirstName + " " + LastName + " (" + EmailAddress + ")";
        }
    }
}
