using InventoryManagement.Api.Modules.Sales.DTOs.OrderItem;
using InventoryManagement.Api.Modules.Sales.Models;

namespace InventoryManagement.Api.Modules.Sales.DTOs.Order
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
