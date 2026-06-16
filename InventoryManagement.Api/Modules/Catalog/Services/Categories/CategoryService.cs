using InventoryManagement.Api.Modules.Catalog.Data;
using InventoryManagement.Api.Modules.Catalog.DTOs.Categories;
using InventoryManagement.Api.Modules.Catalog.Entities;
using InventoryManagement.Api.Shared.Exceptions;
using InventoryManagement.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Catalog.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly CatalogDbContext _context;

        public CategoryService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CategoryResponseDto>> GetPagedAsync(CategoryPageParams @params)
        {
            // inicia o IQueryable
            var query = _context.Categories.AsNoTracking();

            // aplica o filtro de busca se o campo Search foi preenchido
            if (!string.IsNullOrWhiteSpace(@params.Search))
            {
                query = query.Where(category => category.Name.Contains(@params.Search));
            }

            // ordenação dinâmica com padrão "name,asc"
            var sortParts = @params.Sort?.Split(',') ?? ["name", "asc"];
            var property = sortParts[0].ToLower();
            var direction = sortParts.Length > 1 ? sortParts[1].ToLower() : "asc";


            // aplicando a direção (asc / desc) correta
            query = property switch
            {
                "id" => direction == "desc" ? query.OrderByDescending(category => category.Id) : query.OrderBy(category => category.Id),

                "name" => direction == "desc" ? query.OrderByDescending(category => category.Name) : query.OrderBy(category => category.Name),

                _ => direction == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name)
            };

            var totalElements = await query.CountAsync(); // elementos dentro do filtro
            var skip = @params.Skip; // quantos registros pular

            var categories = await query
                .Skip(skip)
                .Take(@params.Size)
                .Select(category => 
                    new CategoryResponseDto(
                        category.Id, 
                        category.Name, 
                        category.Description, 
                        category.CreatedAt, 
                        category.UpdatedAt
                    )).ToListAsync();

            return new PagedResult<CategoryResponseDto>(
                categories, 
                @params.Page, 
                @params.Size, 
                totalElements
            );
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) throw new NotFoundException("Category not found");

            return new CategoryResponseDto(
                category.Id,
                category.Name,
                category.Description,
                category.CreatedAt,
                category.UpdatedAt
            );
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category { Name = dto.Name, Description = dto.Description };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(category.Id);
        }

        public async Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) throw new NotFoundException("Category not found");

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(category.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) throw new NotFoundException("Category not found");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}