using InventoryManagement.Api.Modules.Catalog.DTOs.Categories;
using InventoryManagement.Api.Shared.Pagination;

namespace InventoryManagement.Api.Modules.Catalog.Services.Categories
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryResponseDto>> GetPagedAsync(CategoryPageParams @params);
        Task<CategoryResponseDto?> GetByIdAsync(Guid id);
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
