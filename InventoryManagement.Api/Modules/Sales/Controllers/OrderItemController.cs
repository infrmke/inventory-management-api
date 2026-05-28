using InventoryManagement.Api.Modules.Sales.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Modules.Sales.Controllers
{
    [ApiController]
    [Route("api/sales/order-items")]
    public class OrderItemController : ControllerBase
    {
        public readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orderItems = await _orderItemService.GetAllAsync();
            return Ok(orderItems);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var orderItem = await _orderItemService.GetByIdAsync(id);

            if (orderItem == null) return NotFound(new {error = "Item not found"});

            return Ok(orderItem);
        }
    }
}
