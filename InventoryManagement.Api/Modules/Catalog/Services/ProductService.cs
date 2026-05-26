using InventoryManagement.Api.Modules.Catalog.Data;
using InventoryManagement.Api.Modules.Catalog.DTOs;
using InventoryManagement.Api.Modules.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Catalog.Services
{
    public class ProductService : IProductService
    {
        private readonly CatalogDbContext _context;

        public ProductService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();

            return products.Select(product => new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId
            ));
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return null;

            return new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId
            );
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId
            );
        }
    }
}
