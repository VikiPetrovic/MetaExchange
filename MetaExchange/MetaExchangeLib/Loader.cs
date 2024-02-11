using NJsonSchema;
using System.Text.Json;

namespace MetaExchangeLib
{
    public static class Loader
    {
        public static List<OrderBook> LoadOrderBooks(string orderBooksFilePath)
        {
            List<OrderBook> orderBooks = new List<OrderBook>();

            using (StreamReader sr = new StreamReader(orderBooksFilePath))
            {
                string? line;
                if (sr != null)
                {
                    int i = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //timestamp of orderBook can be ignored
                        string jsonData = line.Split('\t')[1];

                        if (jsonData == null)
                        {
                            continue;
                        }

                        //validate json schema
                        string schemaFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schemas/orderBookSchema.json");
                        string schemaJson = File.ReadAllText(schemaFullPath);

                        var schema = JsonSchema.FromJsonAsync(schemaJson).Result;
                        var result = schema.Validate(jsonData);

                        if (result?.Count > 0)
                        {
                            throw new Exception($"Order book JSON not valid.");
                        }

                        OrderBook? orderBook = JsonSerializer.Deserialize<OrderBook>(jsonData);
                        if (orderBook != null)
                        {
                            // add order book id to the dataset - line ID in incoming data
                            orderBook.Id = i;
                            foreach (OrderBookItem item in orderBook.Asks)
                            {
                                item.Order.OrderBookId = i;
                            }
                            foreach (OrderBookItem item in orderBook.Bids)
                            {
                                item.Order.OrderBookId = i;
                            }

                            orderBooks.Add(orderBook);
                        }
                        else
                        {
                            throw new Exception("Unable to deserialze an order book line (order book is null)");
                        }

                        i++;
                    }
                }
            }

            return orderBooks;

        }
    }
}
