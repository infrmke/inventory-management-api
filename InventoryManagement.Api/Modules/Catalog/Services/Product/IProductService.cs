using InventoryManagement.Api.Modules.Catalog.DTOs;
using InventoryManagement.Api.Modules.Catalog.DTOs.Product;

namespace InventoryManagement.Api.Modules.Catalog.Services.Product
{
    public interface IProductService
    {
        // CRUD:
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
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
