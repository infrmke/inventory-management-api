using InventoryManagement.Api.Modules.Sales.Models;

namespace InventoryManagement.Api.Modules.Sales.DTOs
{
    public record OrderResponseDto(
        Guid Id,
        DateTime OrderDate,
        decimal TotalPrice,
        OrderStatus Status,
        ICollection<OrderItemResponseDto> Items
    );
}
