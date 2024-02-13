using Microsoft.AspNetCore.Mvc;
using MetaExchangeLib;

namespace MetaExchangeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ProcessOrder")]
        public IActionResult ProcessOrder(double btcAmount, string orderType, string userId)
        {
            // Validate inputs
            if (btcAmount < 0)
            {
                return BadRequest("Invalid BTC amount. Non-negative number expected.");
            }

            if (string.IsNullOrWhiteSpace(orderType) || (orderType.ToLower() != "buy" && orderType.ToLower() != "sell"))
            {
                return BadRequest("Invalid order type. Must be 'buy' or 'sell'.");
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                RequestOrder requestOrder = new RequestOrder(UserId: userId, Type: orderType, Amount: btcAmount);      // example: user wants to sell btAmount BTC

                string orderBooksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/order_books_data");
                List<OrderBook> orderBooks = Reader.ReadOrderBooks(orderBooksFile);

                MetaExchange metaExchange = new MetaExchange();                                                                     
                List<Order> results = metaExchange.ProcessRequest(orderBooks, requestOrder);        // the request is processed - a optimal set of order is returned from the order books in the input data set for this request
                                
                return Ok(results.OrderBy(order => order.OrderBookId).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);            }
            
        }
    }
}