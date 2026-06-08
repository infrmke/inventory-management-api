using InventoryManagement.Api.Modules.Sales.Data;
using InventoryManagement.Api.Modules.Sales.DTOs.OrderItem;
using InventoryManagement.Api.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Sales.Services.OrderItem
{
    public class OrderItemService : IOrderItemService
    {
        private readonly SalesDbContext _context;

        public OrderItemService(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItemResponseDto>> GetAllAsync()
        {
            var orderItems = await _context.OrderItems.AsNoTracking().ToListAsync();

            return orderItems.Select(orderItem =>
                new OrderItemResponseDto(
                    orderItem.Id,
                    orderItem.OrderId,
                    orderItem.ProductId,
                    orderItem.Quantity,
                    orderItem.UnitPrice
                ));
        }

        public async Task<OrderItemResponseDto?> GetByIdAsync(Guid id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);

            if (orderItem == null) throw new NotFoundException("Item not found");

            return new OrderItemResponseDto(
                orderItem.Id,
                orderItem.OrderId,
                orderItem.ProductId,
                orderItem.Quantity,
                orderItem.UnitPrice
            );
        }
    }
}