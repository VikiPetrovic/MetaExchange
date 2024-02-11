using System.Text.Json;
using NJsonSchema;

namespace MetaExchangeLib
{
    public class MetaExchange
    {
        public MetaExchange() 
        {
            
        }

        public List<Order> ProcessRequest(List<OrderBook> orderBooks, RequestOrder requestOrder)
        {
            List<Order> resultSet = new List<Order>();

            if (orderBooks == null || orderBooks.Count == 0 || requestOrder == null)
            {
                return resultSet;     //nothing to process
            }

            if (requestOrder.Type.ToLower() != "sell" && requestOrder.Type.ToLower() != "buy")
            {
                throw new Exception($"Unsupported request order type {requestOrder.Type}");
            }

            if (requestOrder.Amount == 0)
            {
                return resultSet;     // nothing to buy/sell
            }

            /*
             * Algorithm that should go over all order books and return the best possible combination of bids/asks depending on the user request
             * */
            if (requestOrder.Type == "sell")
            {
                // user wants to sell N Amount of BTC - check all bids in all order books, ordered in desc to maximize price
                List<OrderBookItem> allBids = orderBooks.SelectMany(orderBook => orderBook.Bids).ToList();
                List<OrderBookItem> orderedBids = allBids.OrderByDescending(bid => bid.Order.Price).ToList();

                double remainingAmount = requestOrder.Amount;

                int index = 0;
                while(remainingAmount > 0.0 && index < orderedBids.Count)       // we finish searching once we fill up the order or we check all available bids/asks in order book
                {
                    Order currentOrder = orderedBids[index].Order;
                    OrderBook b = orderBooks.FirstOrDefault(orderBook => orderBook.Id == currentOrder.OrderBookId);      // to which order book does this order belong?

                    double amount = currentOrder.Amount;
                    double price = currentOrder.Price;

                    double potentialAmount = Math.Min(remainingAmount, currentOrder.Amount);
                    double potentialPrice = potentialAmount * currentOrder.Price;

                    if (b.availableBalanceEUR >= potentialPrice)
                    {
                        // if there is enough EUR balance on this order book, this order can go to the result set
                        resultSet.Add(
                            new Order(type: "buy", price: currentOrder.Price, amount: potentialAmount, orderBookId: currentOrder.OrderBookId)
                            );

                        // available EUR balance of this order book is decreased
                        b.availableBalanceEUR -= potentialPrice;

                        // remaining BTC from request order to cover is decreased
                        remainingAmount -= potentialAmount;
                    }

                    index++;                    
                }
            }

            if (requestOrder.Type == "buy")
            {
                // user wants to buy N Amount of BTC - check all asks in the order books, ordered in asc order to minimize price
                List<OrderBookItem> allAsks = orderBooks.SelectMany(orderBook => orderBook.Asks).ToList();
                List<OrderBookItem> orderedAsks = allAsks.OrderBy(ask => ask.Order.Price).ToList();

                //TODO: buy logic
            }
            return resultSet;
        }

    }
}
