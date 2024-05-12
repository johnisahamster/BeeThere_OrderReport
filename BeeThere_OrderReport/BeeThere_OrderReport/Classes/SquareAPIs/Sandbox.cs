using Square;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BeeThere_OrderReport.Classes.SquareAPIs
{
    internal class Sandbox : SquareEnvironment, ISquareAPI
    {
        public Sandbox()
        {
            GetClient(); //build client
        }

        public ISquareClient GetClient()
        {
            if (client == null)
            {
                try
                {
                    IConfigurationRoot config = GetConfigRoot();
                    application_id = config["AppSettings:Square:SandboxApplicationID"];
                    access_token = config["AppSettings:Square:SandboxAccessToken"];

                    client = new SquareClient.Builder()
                    .BearerAuthCredentials(
                        new Square.Authentication.BearerAuthModel.Builder(
                            access_token
                        )
                        .Build())
                    .Environment(Square.Environment.Sandbox)
                    .Build();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return client;
        }

        public IList<string> GetLocationIDs()
        {
            try
            {
                IConfigurationRoot config = GetConfigRoot();
                string s = config["AppSettings:Square:SandboxLocationID"];
                locationIDs = new List<string>
                {
                    s
                };
            }
            catch (Exception)
            {
                throw;
            }

            return locationIDs;
        }
    }
}
