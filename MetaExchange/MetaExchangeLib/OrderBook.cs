namespace MetaExchangeLib
{
    public class OrderBook
    {
        public int Id { get; set; }
        public DateTime AcqTime { get; set; }
        public List<OrderBookItem> Bids { get; set; }
        public List<OrderBookItem> Asks { get; set; }

        public double balanceEUR { get; set; }
        public double balanceBTC { get; set; }
        public double availableBalanceEUR { get; set; }
        public double availableBalanceBTC { get; set; }

        public OrderBook()
        {
            //generate some random balanca for this exchange - missing in input data
            balanceEUR = 50000; // GenerateEurBalance();
            balanceBTC = 10;    // GenerateBtcBalance();
            // available balance - decreaseing while searching for result
            availableBalanceBTC = balanceBTC;   
            availableBalanceEUR = balanceEUR;
            Bids = new List<OrderBookItem>();
            Asks = new List<OrderBookItem>();
        }

        private double GenerateBtcBalance()
        {
            Random random = new Random();
            return random.NextDouble() * 100;
        }

        private double GenerateEurBalance()
        {
            Random random = new Random();
            return random.NextDouble() * 100000;
        }

        public override string ToString()
        {
            string s = $"\tID: {Id} AcqTime: {AcqTime} EUR Balance: {balanceEUR} BTC Balance: {balanceBTC}\n";

            s += "\tBids:\n";
            foreach (var bid in Bids)
            {
                s += $"\t\t{bid.Order}\n";
            }

            s += "\tAsks:\n";
            foreach (var ask in Asks)
            {
                s += $"\t\t{ask.Order} \n";
            }

            return s;
        }
    }

    

    public class OrderBookItem
    {
        public Order Order { get; set; }
    }

    public class Order
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Kind { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }

        public int OrderBookId { get; set; }

        public Order(string type, double amount, double price, int orderBookId)
        {
            Type = type;
            Amount = amount;
            Price = price;
            OrderBookId = orderBookId;
        }

        public override string ToString()
        {
            return $"Order book id: {OrderBookId} Amount: {Amount}, Price: {Price}, Type: {Type}";
        }
    }
}
