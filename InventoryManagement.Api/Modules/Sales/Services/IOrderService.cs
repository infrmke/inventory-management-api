using InventoryManagement.Api.Modules.Sales.DTOs;

namespace InventoryManagement.Api.Modules.Sales.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetAllAsync();
        Task<OrderResponseDto?> GetByIdAsync(Guid id);
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
        Task<OrderResponseDto?> CancelAsync(Guid id);
    }
}
