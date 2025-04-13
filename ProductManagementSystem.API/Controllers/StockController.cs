using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using ProductManagmentSystem.Application.Messaging;

namespace ProductManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private readonly IStockUpdateProducer _producer;
        public StockController(IStockUpdateProducer producer)
        {
            _producer = producer;
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateStock([FromQuery] int productId, [FromQuery] int newStock)
        {
            await _producer.PublishStockUpdateAsync(productId, newStock);
            return Ok($"Stock update message for product {productId} queued.");
        }
    }
}
