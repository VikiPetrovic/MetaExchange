using MetaExchangeLib;
using NUnit.Framework.Legacy;
using System.Linq.Expressions;

namespace MetaExchangeTests
{
    public class SellRequestTests
    {
        private MetaExchange metaExchange;

        [SetUp]
        public void Setup()
        {
            // Initialize or inject dependencies            
        }

        [Test]
        public void ProcessRequest_EmptyOrderBooks_ReturnsEmptyList()
        {

            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data_empty");
            List<OrderBook> orderBooks =  Reader.ReadOrderBooks(orderBooksFile);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public void ProcessRequest_NullRequestOrder_ReturnsEmptyList()
        {
            // Arrange            
            RequestOrder requestOrder = null;

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public void ProcessRequest_UnsupportedRequestOrderType_ThrowsException()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "SomeType", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile);

            metaExchange = new MetaExchange();

            // Act & Assert
            Assert.Throws<Exception>(() => metaExchange.ProcessRequest(orderBooks, requestOrder));
        }

        [Test]
        public void ProcessRequest_RequestSellAmountZero_ReturnsEmptyList()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 0.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }


        [Test]
        public void ProcessRequest_RequestSellAmount_ReturnsOptimalResult()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 10000, btcBalance: 5);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            List<Order> expected_result = new List<Order>()
            {
                new Order(type: "buy", amount: 1, price:10000, orderBookId: 1),
                new Order(type: "buy", amount: 2, price:2000, orderBookId: 0)
            };

            ClassicAssert.AreEqual(expected_result, result);
        }


        [Test]
        public void ProcessRequest_RequestSellAmount_ReturnsOptimalResult1()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 50000, btcBalance: 5);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            List<Order> expected_result = new List<Order>()
            {
                new Order(type: "buy", amount: 2, price:10000, orderBookId: 1),
                new Order(type: "buy", amount: 1, price:2000, orderBookId: 0)
            };

            ClassicAssert.AreEqual(expected_result, result);
        }


        [Test]
        public void ProcessRequest_RequestSellAmount_LowBalance_ReturnsOptimalResult1()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 500, btcBalance: 5);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            List<Order> expected_result = new List<Order>()
            {
                new Order(type: "buy", amount: 1, price:500, orderBookId: 0),
                new Order(type: "buy", amount: 2, price:200, orderBookId: 1)
            };

            ClassicAssert.AreEqual(expected_result, result);
        }


        [Test]
        public void ProcessRequest_RequestSellAmount_TooLowBalance_ReturnsEmptyList()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 50, btcBalance: 5);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public void ProcessRequest_RequestExceededSellAmount_ReturnsEmptyList()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 50.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 50000, btcBalance: 5);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public void ProcessRequest_RequestSellAmount_ZeroBalance_ReturnsEmptyList()
        {
            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
            List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile, eurBalance: 0, btcBalance: 5);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }
    }
}