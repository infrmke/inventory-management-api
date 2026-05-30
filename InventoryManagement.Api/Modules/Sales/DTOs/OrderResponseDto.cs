using InventoryManagement.Api.Modules.Sales.Models;

namespace InventoryManagement.Api.Modules.Sales.DTOs
{
    public record OrderResponseDto(
        Guid Id,
        decimal TotalPrice,
        OrderStatus Status,
        ICollection<OrderItemResponseDto> Items,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
