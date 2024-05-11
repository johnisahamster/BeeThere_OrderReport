using Square;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeThere_OrderReport.Classes.SquareAPIs
{
    internal interface ISquareAPI
    {
        public abstract ISquareClient GetClient();
    }
}
