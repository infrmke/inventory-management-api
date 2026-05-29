using InventoryManagement.Api.Modules.Sales.Data;
using InventoryManagement.Api.Modules.Sales.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Sales.Services
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

            if (orderItem == null) return null;

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
