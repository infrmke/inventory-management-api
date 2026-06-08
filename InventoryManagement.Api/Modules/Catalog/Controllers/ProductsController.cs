using InventoryManagement.Api.Modules.Catalog.DTOs;
using InventoryManagement.Api.Modules.Catalog.DTOs.Product;
using InventoryManagement.Api.Modules.Catalog.Services.Product;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Modules.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("category/{categoryId:Guid}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            var products = await _productService.GetByCategoryIdAsync(categoryId);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            var updated = await _productService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpPatch("{id:Guid}/adjust-stock")]
        public async Task<IActionResult> AdjustStock(Guid id, [FromBody] AdjustProductStockDto dto)
        {
            var updatedProduct = await _productService.AdjustStockManuallyAsync(id, dto);
            return Ok(updatedProduct);
        }

        [HttpPatch("{id:Guid}/update-price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdateProductPriceDto dto)
        {
            var updatedProduct = await _productService.UpdatePriceAsync(id, dto);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}