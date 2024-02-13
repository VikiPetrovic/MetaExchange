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
            List<Order> potentialCandidates = new List<Order>();

            if (orderBooks == null || orderBooks.Count == 0 || requestOrder == null)
            {
                return potentialCandidates;     //nothing to process
            }

            if (requestOrder.Type.ToLower() != "sell" && requestOrder.Type.ToLower() != "buy")
            {
                throw new Exception($"Unsupported request order type {requestOrder.Type}");
            }

            if (requestOrder.Amount == 0)
            {
                return potentialCandidates;     // nothing to buy/sell
            }

            /*
             * Algorithm that should go over all order books and return the best possible combination of bids/asks depending on the user request and order books balances
             * */
            if (requestOrder.Type == "sell")
            {              
                // user wants to sell N Amount of BTC - check all bids in all order books, ordered in desc to maximize price
                List<OrderBookItem> allBids = orderBooks.SelectMany(orderBook => orderBook.Bids).ToList();
                List<OrderBookItem> orderedBids = allBids.OrderByDescending(bid => bid.Order.Price).ToList();

                double remainingAmount = requestOrder.Amount;                

                Dictionary<int, double> balancesDict = new Dictionary<int, double>();
                foreach (OrderBook ob in orderBooks)
                {
                    balancesDict.Add(ob.Id, ob.availableBalanceEUR);
                }

                int index = 0;
                while(remainingAmount > 0.0 && index < orderedBids.Count)       // we finish searching once we fill up the order or we check all available bids/asks in order book
                {
                    Order order = orderedBids[index].Order;
                    double availableEurBalance = balancesDict[order.OrderBookId];

                    double min1 = Math.Min(order.Amount, remainingAmount);
                    double potentialAmount = Math.Min(min1, (availableEurBalance / order.Price));

                    double potentialPrice = potentialAmount * order.Price;

                    if (availableEurBalance >= potentialPrice)
                    {
                        // if there is enough EUR balance on this order book, this order can go to the result candidates set
                        potentialCandidates.Add(
                            new Order(type: "buy", price: order.Price, amount: potentialAmount, orderBookId: order.OrderBookId)
                            );

                        // available EUR balance of this order book is decreased
                        availableEurBalance -= potentialPrice;
                        balancesDict[order.OrderBookId] = availableEurBalance;

                        // remaining BTC from request order to cover is decreased
                        remainingAmount -= potentialAmount;
                    }

                    //var t = tempOrderBooks.FirstOrDefault(ob => ob.availableBalanceEUR > 0, null);
                    // if we maxxed out all order books but have not managed to sell all users' BTC yet - empty potentialCandidates and start again
                    if (balancesDict.Values.All(value => value == 0) && remainingAmount > 0)
                    {
                        potentialCandidates.Clear();
                        remainingAmount = requestOrder.Amount;
                        balancesDict.Clear();
                        foreach (OrderBook ob in orderBooks)
                        {
                            balancesDict.Add(ob.Id, ob.availableBalanceEUR);
                        }
                        orderedBids.RemoveAt(0);
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }                                  
                }

                // check if potential candidates cover all BTC to sell
                double sum = Math.Round(potentialCandidates.Sum(obj => obj.Amount), 5);
                if (sum != requestOrder.Amount)
                {
                    potentialCandidates.Clear();    // not all BTC can be sold with the budget of these order books - no results returned
                }
            }

            if (requestOrder.Type == "buy")
            {
                // user wants to buy N Amount of BTC - check all asks in the order books, ordered in asc order to minimize price
                List<OrderBookItem> allAsks = orderBooks.SelectMany(orderBook => orderBook.Asks).ToList();
                List<OrderBookItem> orderedAsks = allAsks.OrderBy(ask => ask.Order.Price).ToList();

                double remainingAmount = requestOrder.Amount;

                int index = 0;
                while (remainingAmount > 0.0 && index < orderedAsks.Count)       // we finish searching once we fill up the order or we check all available bids/asks in order book
                {
                    Order currentOrder = orderedAsks[index].Order;
                    OrderBook b = orderBooks.FirstOrDefault(orderBook => orderBook.Id == currentOrder.OrderBookId);      // to which order book does this order belong?

                    double min1 = Math.Min(remainingAmount, currentOrder.Amount);
                    double potentialAmount = Math.Min(min1, b.availableBalanceBTC);

                    double potentialPrice = potentialAmount * currentOrder.Price;

                    if (b.availableBalanceBTC >= potentialAmount)
                    {
                        // if there is enough BTC balance on this order book, this order can go to the result set
                        potentialCandidates.Add(
                            new Order(type: "sell", price: currentOrder.Price, amount: potentialAmount, orderBookId: currentOrder.OrderBookId)
                            );

                        // available BTC balance of this order book is decreased
                        b.availableBalanceBTC -= potentialAmount;

                        // remaining BTC from request order to cover is decreased
                        remainingAmount -= potentialAmount;
                    }

                    index++;
                }
            }
            return potentialCandidates;
        }

    }
}
