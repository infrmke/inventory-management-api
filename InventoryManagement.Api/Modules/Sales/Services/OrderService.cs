using InventoryManagement.Api.Modules.Sales.Data;
using InventoryManagement.Api.Modules.Sales.DTOs;
using InventoryManagement.Api.Modules.Sales.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Sales.Services
{
    public class OrderService : IOrderService
    {
        private readonly SalesDbContext _context;

        public OrderService(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(order => order.Items)
                .AsNoTracking()
                .ToListAsync();

            return orders.Select(order =>
            {
                var itemsDto = order.Items.Select(item =>
                    new OrderItemResponseDto(
                        item.Id,
                        item.OrderId,
                        item.ProductId,
                        item.Quantity,
                        item.UnitPrice
                    )
                ).ToList();

                return new OrderResponseDto(
                    order.Id,
                    order.OrderDate,
                    order.TotalPrice,
                    order.Status,
                    itemsDto
                );
            });
        }

        public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null) return null;

            var itemsDto = order.Items.Select(item =>
                new OrderItemResponseDto(
                    item.Id,
                    item.OrderId,
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice
                )
            ).ToList();

            return new OrderResponseDto(
                order.Id,
                order.OrderDate,
                order.TotalPrice,
                order.Status,
                itemsDto
            );
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
        {
            // cria cada item dentro de uma lista
            var orderItems = dto.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList();

            // instancia um novo pedido com data, status e valor total
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                TotalPrice = orderItems.Sum(item => item.UnitPrice * item.Quantity),
                Status = OrderStatus.Pending,
                Items = orderItems
            };

            // passa a rastrear OrderItem e Order e então os salva no banco
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var itemsDto = order.Items.Select(item =>
                new OrderItemResponseDto(
                    item.Id,
                    item.OrderId,
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice
                )
            ).ToList();

            return new OrderResponseDto(
                order.Id,
                order.OrderDate,
                order.TotalPrice,
                order.Status,
                itemsDto
            );
        }
    }
}
