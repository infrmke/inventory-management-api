using InventoryManagement.Api.Modules.Catalog.Data;
using InventoryManagement.Api.Modules.Catalog.DTOs;
using InventoryManagement.Api.Modules.Catalog.DTOs.Product;
using InventoryManagement.Api.Modules.Catalog.Models;
using InventoryManagement.Api.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Catalog.Services.Product
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
                product.CategoryId,
                product.CreatedAt,
                product.UpdatedAt
            ));
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            return new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId,
                product.CreatedAt,
                product.UpdatedAt
            );
        }

        public async Task<IEnumerable<ProductResponseDto>> GetByCategoryIdAsync(Guid categoryId)
        {
            // verifica se a categoria existe
            var categoryExists = await _context.Categories.AnyAsync(category => category.Id == categoryId);

            if (!categoryExists)
                throw new NotFoundException($"Category with ID {categoryId} not found");

            var products = await _context.Products
                .Where(product => product.CategoryId == categoryId)
                .ToListAsync();

            return products.Select(product => 
                new ProductResponseDto(
                    product.Id, 
                    product.Name, 
                    product.Description, 
                    product.Price, 
                    product.StockQuantity, 
                    product.CategoryId, 
                    product.CreatedAt, 
                    product.UpdatedAt
                )
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

            return await GetByIdAsync(product.Id);
        }

        public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<bool> ReturnStockAsync(Guid id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            // devolve a qtd de estoque ao produto e salva
            product.StockQuantity += quantity;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeductStockAsync(Guid id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            // verifica se há estoque
            if (product.StockQuantity < quantity) return false;

            // reduz a qtd do estoque e salva
            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ProductResponseDto?> AdjustStockManuallyAsync(Guid id, AdjustProductStockDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            // se o ajuste for negativo, garante que o estoque não vá abaixo de zero
            if (product.StockQuantity + dto.Quantity < 0) throw new BadRequestException("Invalid stock quantity");

            // aplica o ajuste e salva
            product.StockQuantity += dto.Quantity;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<ProductResponseDto?> UpdatePriceAsync(Guid id, UpdateProductPriceDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            // ajusta o preço e salva
            product.Price = dto.NewPrice;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) throw new NotFoundException("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}