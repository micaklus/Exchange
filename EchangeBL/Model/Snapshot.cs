namespace ConsoleExchange.Model
{

    public class Snapshot
    {
        public List<OrderWrapper> Bids { get; set; }
        public List<OrderWrapper> Asks { get; set; }
    }
}
