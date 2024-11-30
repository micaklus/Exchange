using ConsoleExchange.Model;
using Newtonsoft.Json;
using System.Text;

namespace Exchange.BL
{
    public class MetaExchange
    {
        public async Task<List<OrderWrapper>> FindBestPossibleOrderToExecute(string filePath, OrderType orderType, decimal amount)
        {

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The file path cannot be null or empty.", nameof(filePath));
            }
            var bestAvailableOrders = new List<OrderWrapper>();

            using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
            {
                int exchangeId = 1;
                using (StreamReader streamReader = new(fs, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        try
                        {
                            var line = streamReader.ReadLine();
                            // Extract JSON data
                            line = ExtractJsonFromLine(line);
                            var snapshot = JsonConvert.DeserializeObject<Snapshot>(line);
                            bestAvailableOrders = UpdateBestOrders(snapshot, bestAvailableOrders, orderType, amount, exchangeId);
                            exchangeId++;
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"JSON parsing error: {ex.Message}");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            // Handle unexpected errors.
                            Console.WriteLine($"An error occurred while processing the line: {ex.Message}");
                        }
                    }
                }
            }
            //Last check if amount big enough
            decimal totalAmount = bestAvailableOrders.Sum(order => order.Order.Amount);
            if (totalAmount != amount)
            {
                throw new InvalidOperationException($"Insufficient liquidity: The total amount available ({totalAmount}) is less than the requested amount ({amount}). Not enough orders available to fully satisfy the request.");
            }
            return bestAvailableOrders;
        }

        private List<OrderWrapper> UpdateBestOrders(Snapshot snapshot, List<OrderWrapper> currentBestOrders, OrderType orderType, decimal amount, int exchangeId)
        {
            var allOrders = orderType == OrderType.Buy
              ? snapshot.Asks.Concat(currentBestOrders) : snapshot.Bids.Concat(currentBestOrders);
            switch (orderType)
            {
                case OrderType.Buy:
                    return GetBestAsksFromExchange(allOrders.ToList(), amount, exchangeId);
                case OrderType.Sell:
                    return GetBestBidsFromExchange(allOrders.ToList(), amount, exchangeId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderType), "Invalid order type.");
            }
        }

        private string ExtractJsonFromLine(string line)
        {
            int jsonStartIndex = line.IndexOf('{');
            return jsonStartIndex >= 0 ? line.Substring(jsonStartIndex) : string.Empty;
        }

        private List<OrderWrapper> GetBestOrdersFromExchange(
            List<OrderWrapper> orders,
            decimal amount,
            int exchangeName,
            Func<IEnumerable<OrderWrapper>, IOrderedEnumerable<OrderWrapper>> sortFunc)
        {
            if (orders == null || orders.Count == 0 || amount <= 0)
            {
                return new List<OrderWrapper>();
            }

            decimal remainingAmount = amount;
            var bestOrders = new List<OrderWrapper>();

            foreach (var order in sortFunc(orders).TakeWhile(_ => remainingAmount > 0))
            {
                decimal amountToTake = Math.Min(order.Order.Amount, remainingAmount);
                bestOrders.Add(new OrderWrapper
                {
                    ExchangeId = order.ExchangeId ?? exchangeName,
                    Order = new Order
                    {
                        Amount = amountToTake,
                        Price = order.Order.Price,
                        Type = order.Order.Type,
                        Kind = order.Order.Kind
                    }
                });
                remainingAmount -= amountToTake;
            }

            return bestOrders;
        }

        public List<OrderWrapper> GetBestBidsFromExchange(List<OrderWrapper> bids, decimal amount, int exchangeName)
        {
            return GetBestOrdersFromExchange(
                bids,
                amount,
                exchangeName,
                orders => orders.OrderByDescending(o => o.Order.Price));
        }

        public List<OrderWrapper> GetBestAsksFromExchange(List<OrderWrapper> asks, decimal amount, int exchangeName)
        {
            return GetBestOrdersFromExchange(
                asks,
                amount,
                exchangeName,
                orders => orders.OrderBy(o => o.Order.Price));
        }
    }
}
