using InventoryManagement.Api.Modules.Catalog.Services.Products;
using InventoryManagement.Api.Modules.Sales.Data;
using InventoryManagement.Api.Modules.Sales.Dtos.OrderItems;
using InventoryManagement.Api.Modules.Sales.Dtos.Orders;
using InventoryManagement.Api.Modules.Sales.Entities;
using InventoryManagement.Api.Shared.Exceptions;
using InventoryManagement.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Sales.Services.Orders
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

        public async Task<PagedResult<OrderResponseDto>> GetPagedAsync(OrderPageParams @params)
        {
            // inicia o IQueryable
            var query = _context.Orders.AsNoTracking();

            // filtro de produto específico
            if (@params.ProductId.HasValue)
            {
                query = query.Where(order => order.Items.Any(item => item.ProductId == @params.ProductId));
            }

            // filtro de preço total mínimo
            if (@params.MinTotal.HasValue)
            {
                query = query.Where(order => order.TotalPrice >= @params.MinTotal);
            }

            // filtro de preço total máximo
            if (@params.MaxTotal.HasValue)
            {
                query = query.Where(order => order.TotalPrice <= @params.MaxTotal);
            }

            // filtro de status
            if (@params.Status.HasValue)
            {
                query = query.Where(order => order.Status == @params.Status);
            }

            // ordenação dinâmica com padrão "createdAt,desc"
            var sortParts = @params.Sort?.Split(",") ?? ["createdAt", "desc"];
            var property = sortParts[0].ToLower();
            var direction = sortParts.Length > 1 ? sortParts[1].ToLower() : "asc";

            // aplicando a direção (asc / desc) correta
            query = property switch
            {
                "id" => direction == "desc" ? query.OrderByDescending(order => order.Id) : query.OrderBy(order => order.Id),

                "totalprice" => direction == "desc" ? query.OrderByDescending(order => order.TotalPrice) : query.OrderBy(order => order.TotalPrice),

                "createdat" => direction == "desc" ? query.OrderByDescending(order => order.CreatedAt) : query.OrderBy(order => order.CreatedAt),

                _ => direction == "desc" ? query.OrderByDescending(order => order.CreatedAt) : query.OrderBy(order => order.CreatedAt)
            };

            var totalElements = await query.CountAsync(); // elementos dentro do filtro
            var skip = @params.Skip; // quantos registros pular

            var orders = await query
                .Skip(skip)
                .Take(@params.Size)
                .Select(order => 
                    new OrderResponseDto(
                        order.Id,
                        order.TotalPrice,
                        order.Status,
                        order.Items.Select(item => new OrderItemResponseDto(
                            item.Id,
                            item.OrderId,
                            item.ProductId,
                            item.Quantity,
                            item.UnitPrice
                        )).ToList(),
                        order.CreatedAt,
                        order.UpdatedAt
                    )).ToListAsync();

            return new PagedResult<OrderResponseDto>(
                orders,
                @params.Page,
                @params.Size,
                totalElements
            );
        }

        public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null) throw new NotFoundException("Order not found");

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

                if (product == null) throw new NotFoundException($"Product with ID {itemDto.ProductId} not found");

                // simula se o estoque suporta a compra
                if (product.StockQuantity < itemDto.Quantity) throw new BadRequestException($"Stock quantity of {itemDto.Quantity} not available for {product.Name}");
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

            if (order == null) throw new NotFoundException("Order not found");
            if (order.Status == OrderStatus.Cancelled) throw new BadRequestException("Order is not active");

            // verifica se o produto existe
            var product = await _productService.GetByIdAsync(dto.ProductId);
            if (product == null) throw new NotFoundException($"Product with ID {dto.ProductId} not found");

            // retira a qtd do estoque
            var stockDeducted = await _productService.DeductStockAsync(dto.ProductId, dto.Quantity);
            if (!stockDeducted) throw new BadRequestException($"Stock quantity of {dto.Quantity} not available");

            // verifica se o item já existe no pedido
            var existingItem = order.Items.FirstOrDefault(item => item.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                // se já existir, soma a nova quantidade
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                // se não, adiciona o novo item
                order.Items.Add(new OrderItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            // recalcula o valor total do pedido e salva
            order.TotalPrice = order.Items.Sum(item => item.UnitPrice * item.Quantity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<OrderResponseDto?> UpdateItemQuantityAsync(Guid id, Guid productId, UpdateOrderItemQuantityDto dto)
        {
            var order = await _context.Orders.Include(order => order.Items).FirstOrDefaultAsync(order => order.Id == id);

            if (order == null) throw new NotFoundException("Order not found");
            if (order.Status == OrderStatus.Cancelled) throw new BadRequestException("Order is not active");

            var item = order.Items.FirstOrDefault(item => item.ProductId == productId);

            if (item == null) throw new NotFoundException("Product not found");

            // lógica de estoque abaixo
            int difference = dto.Quantity - item.Quantity;

            if (difference > 0)
            {
                // deduz a diferença do estoque se a diff for maior
                var stockDeducted = await _productService.DeductStockAsync(productId, difference);

                if (!stockDeducted) throw new BadRequestException($"Stock quantity of {dto.Quantity} not available");
            }
            else if (difference < 0)
            {
                // devolve qtd ao estoque se a diff for menor
                await _productService.ReturnStockAsync(productId, Math.Abs(difference));
            }

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

            if (order == null) throw new NotFoundException("Order not found");
            if (order.Status == OrderStatus.Cancelled) throw new BadRequestException("Order is not active");

            var item = order.Items.FirstOrDefault(item => item.ProductId == productId);

            if (item == null) throw new NotFoundException("Product not found");

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

            if (order == null) throw new NotFoundException("Order not found");
            if (order.Status == OrderStatus.Cancelled) throw new BadRequestException("Order is already inactive");

            // altera o status e salva
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            // varre os itens do pedido e os devolve ao estoque
            foreach (var item in order.Items)
            {
                await _productService.ReturnStockAsync(item.ProductId, item.Quantity);
            }

            return await GetByIdAsync(order.Id);
        }
    }
}