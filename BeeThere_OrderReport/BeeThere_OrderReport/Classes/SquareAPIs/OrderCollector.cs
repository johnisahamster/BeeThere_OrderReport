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

        private ISquareClient client;
        public ISquareClient Client { get { return client; } }

        
        public async Task GetOrders(Queue<Order> orders, DateTime from, DateTime to, bool dec = false)
        {
            IOrdersApi ordersapi = client.OrdersApi;

            SearchOrdersFilter filter = new SearchOrdersFilter.Builder()
                .DateTimeFilter(new SearchOrdersDateTimeFilter.Builder()
                    .CreatedAt(new TimeRange.Builder()
                        .StartAt(from.ToLongDateString())
                        .EndAt(to.ToLongDateString())
                        .Build()
                    ).Build()
                ).Build();
            SearchOrdersSort sort = new SearchOrdersSort.Builder("CREATED_AT")
                .SortOrder((dec) ? "DEC" : "ASC")
                .Build();

            SearchOrdersQuery query = new SearchOrdersQuery(filter, sort);

            SearchOrdersRequest request = new SearchOrdersRequest.Builder()
                .Query(query)
                .ReturnEntries(false)
                .Build();
            
            //TODO: implement cancellation token
            CancellationToken cancellationToken = new CancellationToken();
            SearchOrdersResponse response;

            try
            {
                response = await ordersapi.SearchOrdersAsync(request, cancellationToken);
                Console.WriteLine("Successfully called SearchOrders");
            }
            catch (ApiException e)
            {
                string message = "ERROR! Something happened while trying to reach the Square API:\n" + e.Message;
                MessageDialog dialog = new MessageDialog(message);
                dialog.Title = "Square API Exception";
                await dialog.ShowAsync();
                throw;
            }

            if (response == null || response.Orders.Count == 0)
            {
                string message = "No orders returned.";
                MessageDialog dialog = new MessageDialog(message);
                await dialog.ShowAsync();
                return;
            }

            orders = new Queue<Order>(response.Orders);
            return;
        }
        public async Task GetOrders(Queue<Order> orders, bool dec = false) { await GetOrders(orders, DateTime.Now.AddMonths(1), DateTime.Now, dec); }
        
    }
}
