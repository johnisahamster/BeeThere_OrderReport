using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeThere_OrderReport.Classes.Square
{
    class SquareEnvironment
    {
        public SquareEnvironment() { }

        protected string application_id;
        protected string access_token;

        public string GetAppID() { return application_id; }
        public string GetAccessToken() { return access_token; }
    }
}
