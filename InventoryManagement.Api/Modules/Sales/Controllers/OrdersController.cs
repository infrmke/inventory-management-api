using InventoryManagement.Api.Modules.Sales.DTOs.Order;
using InventoryManagement.Api.Modules.Sales.DTOs.OrderItem;
using InventoryManagement.Api.Modules.Sales.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var orders = await _orderService.GetByIdAsync(id);
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
            return Ok(updatedOrder);
        }

        [HttpPatch("{id:Guid}/items/{productId:Guid}")]
        public async Task<IActionResult> UpdateItemQuantity(Guid id, Guid productId, [FromBody] UpdateOrderItemQuantityDto dto)
        {
            var updatedOrder = await _orderService.UpdateItemQuantityAsync(id, productId, dto);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:Guid}/items/{productId:Guid}")]
        public async Task<IActionResult> RemoveItem(Guid id, Guid productId)
        {
            var updatedOrder = await _orderService.RemoveItemAsync(id, productId);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var cancelled = await _orderService.CancelAsync(id);
            return Ok(cancelled);
        }
    }
}