namespace InventoryManagement.Api.Modules.Catalog.DTOs
{
    public record ProductResponseDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        Guid CategoryId
    );
}
