namespace MetaExchangeLib
{
    public class RequestOrder
    {
        public string UserId { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }

        public RequestOrder(string UserId, string Type, double Amount)
        {
            this.UserId = UserId;
            this.Type = Type;
            this.Amount = Amount;
        }

        public override string ToString()
        {
            return $"UserId: {UserId}, Type: {Type}, Amount: {Amount}";
        }
    }
}
