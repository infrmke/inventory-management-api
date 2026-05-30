using InventoryManagement.Api.Modules.Catalog.DTOs;

namespace InventoryManagement.Api.Modules.Catalog.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto);
        Task<bool> ReturnStockAsync(Guid id, int quantity);
        Task<bool> DeleteAsync(Guid id);
    }
}
