using InventoryManagement.Api.Modules.Catalog.DTOs;

namespace InventoryManagement.Api.Modules.Catalog.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(Guid id);
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
    }
}
