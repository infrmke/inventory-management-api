namespace InventoryManagement.Api.Modules.Catalog.Dtos.Categories
{
    public record CategoryResponseDto(
        Guid Id, 
        string Name, 
        string Description,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
