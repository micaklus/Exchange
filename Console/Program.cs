using ConsoleExchange.Model;
using Exchange.BL;

namespace ConsoleExchange
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the Boerse Stuttgart Digital!");

            while (true)
            {
                Console.WriteLine("\nDo you want to buy or sell Bitcoin?");
                Console.WriteLine("1. Buy");
                Console.WriteLine("2. Sell");
                Console.WriteLine("3. Exit");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            await BuyBitcoin();
                            break;
                        case "2":
                            await SellBitcoin();
                            break;
                        case "3":
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        static async Task BuyBitcoin()
        {
            Console.WriteLine("\nEnter the amount of Bitcoin you want to buy:");
            if (double.TryParse(Console.ReadLine(), out double amount) && amount > 0)
            {
                Console.WriteLine($"Buying {amount} Bitcoin...");
                var metaExchange = new MetaExchange();
                var orders = await metaExchange.FindBestPossibleOrderToExecute("Assets/order_books_data", OrderType.Buy, (decimal)amount);
                WriteOrders(orders);
            }
            else
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
            }
        }
        private static void WriteOrders(List<OrderWrapper> orders)
        {
            Console.WriteLine("List of best orders(" + orders.Count + ")");
            foreach (OrderWrapper order in orders)
            {
                Console.WriteLine("##############################");
                Console.WriteLine("Exchange:" + order.ExchangeId);
                Console.WriteLine("Price:" + order.Order.Price);
                Console.WriteLine("Amount:" + order.Order.Amount);
                Console.WriteLine("##############################");
            }
            Console.WriteLine("Price " + Math.Round(orders.Sum(item => item.Order.Amount * item.Order.Price), 2) + "EUR");



        }
        static async Task SellBitcoin()
        {
            Console.WriteLine("\nEnter the amount of Bitcoin you want to sell:");
            if (double.TryParse(Console.ReadLine(), out double amount) && amount > 0)
            {
                Console.WriteLine($"Selling {amount} Bitcoin...");
                var metaExchange = new MetaExchange();
                var orders = await metaExchange.FindBestPossibleOrderToExecute("Assets/order_books_data", OrderType.Sell, (decimal)amount);
                WriteOrders(orders);
            }
            else
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
            }
        }
    }
}