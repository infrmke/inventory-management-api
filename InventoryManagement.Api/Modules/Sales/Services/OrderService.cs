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
            // verificação precoce de produto e estoque
            foreach (var itemDto in dto.Items)
            {
                var product = await _productService.GetByIdAsync(itemDto.ProductId);

                if (product == null) return null;

                // simula se o estoque suporta a compra
                if (product.StockQuantity < itemDto.Quantity) return null;
            }

            // instancia cada item
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in dto.Items)
            {
                await _productService.DeductStockAsync(itemDto.ProductId, itemDto.Quantity);

                var product = await _productService.GetByIdAsync(itemDto.ProductId);

                orderItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });
            }

            // instancia o pedido
            var order = new Order
            {
                Status = OrderStatus.Pending,
                Items = orderItems,
                TotalPrice = orderItems.Sum(item => item.UnitPrice * item.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(order.Id); // devolve o pedido atualizado
        }

        public async Task<OrderResponseDto?> AddItemAsync(Guid id, AddOrderItemDto dto)
        {
            var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == id);

            if (order == null || order.Status == OrderStatus.Cancelled) return null;

            // verifica se há estoque disponível para a qtd do item
            var stockDeducted = await _productService.DeductStockAsync(dto.ProductId, dto.Quantity);

            if (!stockDeducted) return null;

            // verifica se o item já existe no pedido
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

            // recalcula o valor total do pedido e salva
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

            // lógica de estoque abaixo
            int difference = dto.Quantity - item.Quantity;

            if (difference > 0)
            {
                // deduz a diferença do estoque se a diff for maior
                var stockDeducted = await _productService.DeductStockAsync(productId, difference);

                if (!stockDeducted) return null;
            }
            else if (difference < 0)
            {
                // devolve qtd ao estoque se a diff for menor
                await _productService.ReturnStockAsync(productId, Math.Abs(difference));
            }

            // recalcula o valor total do pedido
            order.TotalPrice = order.Items.Sum(item => item.UnitPrice * item.Quantity);

            // atualiza a qtd
            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id); // devolve o pedido atualizado
        }

        public async Task<OrderResponseDto?> RemoveItemAsync(Guid id, Guid productId)
        {
            var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == id);

            if (order == null || order.Status == OrderStatus.Cancelled) return null;

            var item = order.Items.FirstOrDefault(item => item.ProductId == productId);
            
            if (item == null) return null;

            // remove o item e devolve a qtd ao estoque
            order.Items.Remove(item);
            await _productService.ReturnStockAsync(productId, item.Quantity);

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
