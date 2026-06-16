using InventoryManagement.Api.Shared.Pagination.Dtos;

namespace InventoryManagement.Api.Modules.Catalog.Dtos.Products
{
    public record ProductPageParams(
        string? Search,
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        int Page = 0,
        int Size = 10,
        string? Sort = "name,asc"
    ) : BasePageParams(Page, Size, Sort);
}
