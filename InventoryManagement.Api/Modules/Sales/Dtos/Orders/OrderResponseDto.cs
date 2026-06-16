using InventoryManagement.Api.Modules.Sales.Dtos.OrderItems;
using InventoryManagement.Api.Modules.Sales.Entities;

namespace InventoryManagement.Api.Modules.Sales.Dtos.Orders
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
