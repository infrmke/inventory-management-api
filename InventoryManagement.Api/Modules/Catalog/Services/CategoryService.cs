using InventoryManagement.Api.Modules.Catalog.Data;
using InventoryManagement.Api.Modules.Catalog.DTOs;
using InventoryManagement.Api.Modules.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CatalogDbContext _context;

        public CategoryService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            return categories.Select(category => new CategoryResponseDto(category.Id, category.Name, category.Description));
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return null;

            return new CategoryResponseDto(
                category.Id,
                category.Name,
                category.Description
            );
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category { Name = dto.Name, Description = dto.Description };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto(category.Id, category.Name, category.Description);
        }

        public async Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            return new CategoryResponseDto(
                category.Id,
                category.Name,
                category.Description
            );
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
