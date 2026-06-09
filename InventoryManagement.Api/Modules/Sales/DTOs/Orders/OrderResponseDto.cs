using InventoryManagement.Api.Modules.Sales.DTOs.OrderItems;
using InventoryManagement.Api.Modules.Sales.Entities;

namespace InventoryManagement.Api.Modules.Sales.DTOs.Orders
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
