using InventoryManagement.Api.Modules.Catalog.DTOs.Products;
using InventoryManagement.Api.Shared.Pagination;

namespace InventoryManagement.Api.Modules.Catalog.Services.Products
{
    public interface IProductService
    {
        // CRUD:
        Task<PagedResult<ProductResponseDto>> GetPagedAsync(ProductPageParams @params);
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteAsync(Guid id);

        // TASK-BASED:
        Task<IEnumerable<ProductResponseDto>> GetByCategoryIdAsync(Guid categoryId);
        Task<bool> ReturnStockAsync(Guid id, int quantity);
        Task<bool> DeductStockAsync(Guid id, int quantity);
        Task<ProductResponseDto?> AdjustStockManuallyAsync(Guid id, AdjustProductStockDto dto);
        Task<ProductResponseDto?> UpdatePriceAsync(Guid id, UpdateProductPriceDto dto);
    }
}
