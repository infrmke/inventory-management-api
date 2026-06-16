namespace InventoryManagement.Api.Shared.Pagination.Dtos
{
    public record BasePageParams(
        int Page = 0,
        int Size = 10,
        string? Sort = "name,asc"
    )
    {
        public int Skip => Page * Size;
    }
}
