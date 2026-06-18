using InventoryManagement.Api.Modules.Sales.Entities;
using InventoryManagement.Api.Shared.Pagination.Dtos;

namespace InventoryManagement.Api.Modules.Sales.Dtos.Orders
{
    public record OrderPageParams(
        Guid? ProductId,
        decimal? MinTotal,
        decimal? MaxTotal,
        OrderStatus? Status,
        int Page = 0,
        int Size = 10,
        string? Sort = "createdAt,desc"
    ) : BasePageParams(Page, Size, Sort);
}
