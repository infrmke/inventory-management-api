using InventoryManagement.Api.Modules.Sales.DTOs;
using InventoryManagement.Api.Modules.Sales.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Modules.Sales.Controllers
{
    [ApiController]
    [Route("api/sales/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var orders = await _orderService.GetByIdAsync(id);

            if (orders == null) return NotFound(new { error = "Order not found" });

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var result = await _orderService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id:Guid}/items")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddOrderItemDto dto)
        {
            var updatedOrder = await _orderService.AddItemAsync(id, dto);

            if (updatedOrder == null) return NotFound(new { error = "Order not found or is inactive" });

            return Ok(updatedOrder);
        }

        [HttpPatch("{id:Guid}/items/{productId:Guid}")]
        public async Task<IActionResult> UpdateItemQuantity(Guid id, Guid productId, [FromBody] UpdateOrderItemQuantityDto dto)
        {
            var updatedOrder = await _orderService.UpdateItemQuantityAsync(id, productId, dto);

            if (updatedOrder == null) return NotFound(new { error = "Order or product not found, or order is inactive" });
            
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:Guid}/items/{productId:Guid}")]
        public async Task<IActionResult> RemoveItem(Guid id, Guid productId)
        {
            var updatedOrder = await _orderService.RemoveItemAsync(id, productId);

            if (updatedOrder == null) return NotFound(new { error = "Order or product not found, or order is inactive" });
            
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var cancelled = await _orderService.CancelAsync(id);

            if (cancelled == null) return NotFound(new { error = "Order not found OR cannot be cancelled" });

            return Ok(cancelled);
        }
    }
}
