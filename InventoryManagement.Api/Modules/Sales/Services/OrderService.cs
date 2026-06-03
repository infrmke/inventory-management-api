using InventoryManagement.Api.Modules.Catalog.Models;
using InventoryManagement.Api.Modules.Catalog.Services;
using InventoryManagement.Api.Modules.Sales.Data;
using InventoryManagement.Api.Modules.Sales.DTOs;
using InventoryManagement.Api.Modules.Sales.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Sales.Services
{
    public class OrderService : IOrderService
    {
        private readonly SalesDbContext _context;
        private readonly IProductService _productService;

        public OrderService(SalesDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
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
                    order.TotalPrice,
                    order.Status,
                    itemsDto,
                    order.CreatedAt,
                    order.UpdatedAt
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
                order.TotalPrice,
                order.Status,
                itemsDto,
                order.CreatedAt,
                order.UpdatedAt
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
                order.TotalPrice,
                order.Status,
                itemsDto,
                order.CreatedAt,
                order.UpdatedAt
            );
        }

        public async Task<OrderResponseDto?> AddItemAsync(Guid id, AddOrderItemDto dto)
        {
            var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == id);

            if (order == null || order.Status == OrderStatus.Cancelled) return null;

            // verifica se o item já existe
            var existingItem = order.Items.FirstOrDefault(item => item.ProductId == dto.ProductId);

            // se já existir, soma a nova quantidade
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {   
                // caso não, adiciona o item ao pedido
                order.Items.Add(new OrderItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitPrice
                });
            }

            // recalcula o valor total do pedido
            order.TotalPrice = order.Items.Sum(item => item.UnitPrice * item.Quantity);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id); // devolve o pedido atualizado
        }

        public async Task<OrderResponseDto?> UpdateItemQuantityAsync(Guid id, Guid productId, UpdateOrderItemQuantityDto dto)
        {
            var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == id);

            if (order == null || order.Status == OrderStatus.Cancelled) return null;

            var item = order.Items.FirstOrDefault(item => item.ProductId == productId);

            if (item == null) return null;

            // atualiza a qtd
            item.Quantity = dto.Quantity;

            // recalcula o valor total do pedido
            order.TotalPrice = order.Items.Sum(item => item.UnitPrice * item.Quantity);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id); // devolve o pedido atualizado
        }

        public async Task<OrderResponseDto?> RemoveItemAsync(Guid id, Guid productId)
        {
            var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == id);

            if (order == null || order.Status == OrderStatus.Cancelled) return null;

            var item = order.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (item == null) return null;

            // remove o item
            order.Items.Remove(item);

            // recalcula o valor total do pedido
            order.TotalPrice = order.Items.Sum(item => item.UnitPrice * item.Quantity);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id); // devolve o pedido atualizado
        }

        public async Task<OrderResponseDto?> CancelAsync(Guid id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            // não faz nada se o pedido já estiver cancelado
            if (order.Status == OrderStatus.Cancelled) return null;

            // altera o status e salva
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            // varre os itens do pedido e os devolve ao estoque
            foreach (var item in order.Items)
            {
                await _productService.ReturnStockAsync(item.ProductId, item.Quantity);
            }

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
                order.TotalPrice,
                order.Status,
                itemsDto,
                order.CreatedAt,
                order.UpdatedAt
            );
        }
    }
}
