namespace InventoryManagement.Api.Modules.Sales.DTOs.OrderItems
{
    public record OrderItemResponseDto(
        Guid Id,
        Guid OrderId,
        Guid ProductId,
        int Quantity,
        decimal UnitPrice
    );
}
