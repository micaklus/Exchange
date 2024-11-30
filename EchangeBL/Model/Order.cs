namespace ConsoleExchange.Model
{
    public class Order
    {
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public OrderType Type { get; set; }
        public OrderKind Kind { get; set; }
    }
}
