// See https://aka.ms/new-console-template for more information
using MetaExchangeLib;


RequestOrder requestOrderSell = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);      // example: user wants to sell 3 BTC at max price
RequestOrder requestOrderBuy = new RequestOrder(UserId: "1234", Type: "buy", Amount: 5.0);      // example: user wants to buy 5 BTC at min price
string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data_full");

try
{
    List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 500000, btcBalance: 1000);

    MetaExchange metaExchange = new MetaExchange();

    List<Order> results;
    results = metaExchange.ProcessRequest(orderBooks, requestOrderSell);        // the request is processed - a optimal set of order is returned from the order books in the input data set for this request
    //results = metaExchange.ProcessRequest(orderBooks, requestOrderBuy);        // the request is processed - a optimal set of order is returned from the order books in the input data set for this request

    foreach (Order order in results.OrderBy(order => order.OrderBookId).ToList())
    {
        Console.WriteLine(order);
        Console.WriteLine($"Total price {results.Sum(order => order.Price)}");
    }
}
catch (Exception e)
{
    Console.WriteLine($"Error occured while trying to process the request. \nError: {e.Message}\n{e.StackTrace}");
}