namespace InventoryManagement.Api.Modules.Sales.Dtos.OrderItems
{
    public record OrderItemResponseDto(
        Guid Id,
        Guid OrderId,
        Guid ProductId,
        int Quantity,
        decimal UnitPrice
    );
}
