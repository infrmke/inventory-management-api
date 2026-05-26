using InventoryManagement.Api.Modules.Catalog.DTOs;

namespace InventoryManagement.Api.Modules.Catalog.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    }
}
