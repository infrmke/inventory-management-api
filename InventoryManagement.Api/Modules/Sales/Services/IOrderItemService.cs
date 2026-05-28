using InventoryManagement.Api.Modules.Sales.DTOs;

namespace InventoryManagement.Api.Modules.Sales.Services
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemResponseDto>> GetAllAsync();
        Task<OrderItemResponseDto?> GetByIdAsync(Guid id);
    }
}
