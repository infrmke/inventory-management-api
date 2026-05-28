using InventoryManagement.Api.Modules.Sales.Models;

namespace InventoryManagement.Api.Modules.Sales.DTOs
{
    public record OrderItemResponseDto(
        Guid Id,
        Guid OrderId,
        Guid ProductId,
        int Quantity,
        decimal UnitPrice
    );
}
