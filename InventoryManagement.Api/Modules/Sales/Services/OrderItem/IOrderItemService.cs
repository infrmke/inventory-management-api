using InventoryManagement.Api.Modules.Sales.DTOs.OrderItem;

namespace InventoryManagement.Api.Modules.Sales.Services.OrderItem
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemResponseDto>> GetAllAsync();
        Task<OrderItemResponseDto?> GetByIdAsync(Guid id);
    }
}
