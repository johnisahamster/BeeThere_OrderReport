using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Square;
using Square.Apis;
using Square.Exceptions;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace BeeThere_OrderReport.Classes.SquareAPIs
{
    internal class OrderCollector
    {
        public OrderCollector(ISquareClient client) 
        {
            this.client = client;
        }

        private readonly ISquareClient client;
        public ISquareClient Client { get { return client; } }

        
        public async Task<Queue<Order>> GetOrders(XamlRoot xamlRoot, IList<string> locationIDs, DateTime from, DateTime to, bool dec = false)
        {
            IOrdersApi ordersapi = client.OrdersApi;

            SearchOrdersFilter filter = new SearchOrdersFilter.Builder()
                .DateTimeFilter(new SearchOrdersDateTimeFilter.Builder()
                    .CreatedAt(new TimeRange.Builder()
                        .StartAt(from.ToShortDateString())
                        .EndAt(to.ToShortDateString())
                        .Build()
                    ).Build()
                ).Build();
            SearchOrdersSort sort = new SearchOrdersSort.Builder("CREATED_AT")
                .SortOrder((dec) ? "DEC" : "ASC")
                .Build();

            SearchOrdersQuery query = new(filter, sort);

            SearchOrdersRequest request = new SearchOrdersRequest.Builder()
                .Query(query)
                .LocationIds(locationIDs)
                .ReturnEntries(false)
                .Build();
            
            //TODO: implement cancellation token
            //CancellationToken cancellationToken = new();
            SearchOrdersResponse response;

            try
            {
                response = await ordersapi.SearchOrdersAsync(request);
                System.Diagnostics.Debug.WriteLine("Successfully called SearchOrders");
            }
            catch (ApiException e)
            {
                string message = "ERROR! Something happened while trying to reach the Square API:\n";
                await ContentDialogMaker.APIError(xamlRoot, e, message, "Square API Exception");
                throw;
            }

            if (response == null || response.Orders == null || response.Orders.Count == 0)
            {
                await ContentDialogMaker.Run(xamlRoot, "No orders returned.");
                return new Queue<Order>();
            }

            return new Queue<Order>(response.Orders);
        }
        public async Task<Queue<Order>> GetOrders(XamlRoot xamlRoot, IList<string> locationIDs, bool dec = false) { return await GetOrders(xamlRoot, locationIDs, DateTime.Now.AddMonths(-1).AddDays(1), DateTime.Now.AddDays(1), dec); }
        
    }
}
