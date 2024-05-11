using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.Models;
using Square.Exceptions;
using Square.Authentication;
using Microsoft.Extensions.Configuration;

namespace BeeThere_OrderReport.Classes.SquareAPIs
{
    internal class SquareEnvironment
    {
        private static IConfigurationRoot config; //project configuration

        public SquareEnvironment() { }

        protected string application_id;
        protected string access_token;
        protected ISquareClient client;

        public string GetAppID() { return application_id; }
        public string GetAccessToken() { return access_token; }
        
        protected static IConfigurationRoot GetConfigRoot()
        {
            if (config == null)
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
                config = builder.Build();
            }
            return config;
        }
    }
}
