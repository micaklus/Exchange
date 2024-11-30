using ConsoleExchange.Model;
using Exchange.BL;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Model;
namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "0 = Buy and 1 = Sell")]
        [ProducesResponseType(typeof(IEnumerable<OrderWrapper>), 200)]

        public async Task<IActionResult> Post([FromBody] RequestOrder order)
        {
            try
            {
                var metaExchange = new MetaExchange();
                var items = await metaExchange.FindBestPossibleOrderToExecute("Assets/order_books_data", order.Type, order.Amount);
                return items != null ? Ok(items) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
