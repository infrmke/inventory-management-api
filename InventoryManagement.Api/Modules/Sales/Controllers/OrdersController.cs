using InventoryManagement.Api.Modules.Sales.Dtos.Orders;
using InventoryManagement.Api.Modules.Sales.Dtos.OrderItems;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Api.Modules.Sales.Services.Orders;
using InventoryManagement.Api.Shared.Filters;

namespace InventoryManagement.Api.Modules.Sales.Controllers
{
    [ApiController]
    [Route("api/sales/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> GetById(string id)
        {
            var orders = await _orderService.GetByIdAsync(Guid.Parse(id));
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var result = await _orderService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id}/items")]
        [ValidateGuid("id")]
        public async Task<IActionResult> AddItem(string id, [FromBody] AddOrderItemDto dto)
        {
            var updatedOrder = await _orderService.AddItemAsync(Guid.Parse(id), dto);
            return Ok(updatedOrder);
        }

        [HttpPatch("{id}/items/{productId}")]
        [ValidateGuid("id")]
        [ValidateGuid("productId")]
        public async Task<IActionResult> UpdateItemQuantity(string id, string productId, [FromBody] UpdateOrderItemQuantityDto dto)
        {
            var orderGuid = Guid.Parse(id);
            var productGuid = Guid.Parse(productId);

            var updatedOrder = await _orderService.UpdateItemQuantityAsync(orderGuid, productGuid, dto);

            return Ok(updatedOrder);
        }

        [HttpDelete("{id}/items/{productId}")]
        [ValidateGuid("id")]
        [ValidateGuid("productId")]
        public async Task<IActionResult> RemoveItem(string id, string productId)
        {
            var orderGuid = Guid.Parse(id);
            var productGuid = Guid.Parse(productId);

            var updatedOrder = await _orderService.RemoveItemAsync(orderGuid, productGuid);

            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> Cancel(string id)
        {
            var cancelled = await _orderService.CancelAsync(Guid.Parse(id));
            return Ok(cancelled);
        }
    }
}