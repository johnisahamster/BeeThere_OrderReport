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

        public string getAppID() { return application_id; }
        public string getAccessToken() { return access_token; }
    }
}
