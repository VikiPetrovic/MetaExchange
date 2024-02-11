using MetaExchangeLib;
using NUnit.Framework.Legacy;

namespace MetaExchangeTests
{
    public class Tests
    {
        private MetaExchange metaExchange;

        [SetUp]
        public void Setup()
        {
            // Initialize or inject dependencies            
        }

        [Test]
        public void LoadOrderBooks_InvalidJsonOrderBooks_ThrowsException()
        {
            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data_invalidJson");

            // Act & Assert
            Assert.Throws<Exception>(() => Loader.LoadOrderBooks(orderBooksFile));
        }

        [Test]
        public void ProcessRequest_EmptyOrderBooks_ReturnsEmptyList()
        {

            // Arrange            
            RequestOrder requestOrder = new RequestOrder(UserId: "1234", Type: "sell", Amount: 3.0);

            string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data_empty");
            List<OrderBook> orderBooks =  Loader.LoadOrderBooks(orderBooksFile);

            metaExchange = new MetaExchange();

            // Act
            var result = metaExchange.ProcessRequest(orderBooks, requestOrder);

            // Assert
            ClassicAssert.IsEmpty(result);
        }

        

        /*
         * TODO: other tests:
         * process request - 
         * */
    }
}