namespace InventoryManagement.Api.Modules.Catalog.DTOs
{
    public record ProductResponseDto(
        int Id,
        string Name,
        string Description,
        decimal Price,
        int Stock,
        int CategoryId
    );
}
