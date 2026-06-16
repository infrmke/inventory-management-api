namespace InventoryManagement.Api.Modules.Catalog.DTOs.Categories
{
    public record CategoryPageParams(
        string? Search,
        int Page = 0,
        int Size = 10,
        String? Sort = "name,asc"
    );
}
