using MetaExchangeLib;
using NUnit.Framework.Legacy;
using System.Linq.Expressions;

namespace MetaExchangeTests
{
    public class LoadTests
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
            Assert.Throws<Exception>(() => Reader.ReadOrderBooks(orderBooksFile));
        }
    }
}