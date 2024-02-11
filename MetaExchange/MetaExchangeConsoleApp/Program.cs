// See https://aka.ms/new-console-template for more information
using MetaExchangeLib;

RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);      // example: user wants to sell btAmount BTC
string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");

try
{
    List<OrderBook> orderBooks = Loader.LoadOrderBooks(orderBooksFile);
    MetaExchange metaExchange = new MetaExchange();      // exchange is created and order books are loaded and format validated    
    List<Order> results = metaExchange.ProcessRequest(orderBooks, requestOrder);        // the request is processed - a optimal set of order is returned from the order books in the input data set for this request

    foreach (Order order in results.OrderBy(order => order.OrderBookId).ToList())
    {
        Console.WriteLine(order);
    }
}
catch(Exception e)
{
    Console.WriteLine($"Error occured while trying to process thw request {requestOrder}. \nError: {e.Message}\n{e.StackTrace}");
}