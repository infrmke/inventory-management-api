namespace InventoryManagement.Api.Shared.Pagination
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Content { get; set; } = [];

        public int Page { get; set; }

        public int Size { get; set; }

        public long TotalElements { get; set; }

        public int TotalPages => Size > 0 ? (int)Math.Ceiling((double)TotalElements / Size) : 0;

        public PagedResult(IEnumerable<T> content, int page, int size, long totalElements)
        {
            Content = content;
            Page = page;
            Size = size;
            TotalElements = totalElements;
        }
    }
}
