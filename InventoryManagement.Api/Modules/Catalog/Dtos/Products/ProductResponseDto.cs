namespace InventoryManagement.Api.Modules.Catalog.Dtos.Products
{
    public record ProductResponseDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        Guid CategoryId,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
