using InventoryManagement.Api.Modules.Sales.Dtos.Orders;
using InventoryManagement.Api.Modules.Sales.Dtos.OrderItems;

namespace InventoryManagement.Api.Modules.Sales.Services.Orders
{
    public interface IOrderService
    {
        // CRUD:
        Task<IEnumerable<OrderResponseDto>> GetAllAsync();
        Task<OrderResponseDto?> GetByIdAsync(Guid id);
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
        Task<OrderResponseDto?> CancelAsync(Guid id);

        // TASK-BASED:
        Task<OrderResponseDto?> AddItemAsync(Guid id, AddOrderItemDto dto);
        Task<OrderResponseDto?> UpdateItemQuantityAsync(Guid orderId, Guid productId, UpdateOrderItemQuantityDto dto);
        Task<OrderResponseDto?> RemoveItemAsync(Guid orderId, Guid productId);
    }
}
