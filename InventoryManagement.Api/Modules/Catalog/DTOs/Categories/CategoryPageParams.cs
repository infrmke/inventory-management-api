using InventoryManagement.Api.Shared.Pagination.Dtos;

namespace InventoryManagement.Api.Modules.Catalog.DTOs.Categories
{
    public record CategoryPageParams(
        string? Search,
        int Page = 0,
        int Size = 10,
        string? Sort = "name,asc"
    ) : BasePageParams(Page, Size, Sort);
}
