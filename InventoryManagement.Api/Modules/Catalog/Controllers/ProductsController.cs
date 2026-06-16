using InventoryManagement.Api.Modules.Catalog.Dtos.Products;
using InventoryManagement.Api.Modules.Catalog.Services.Products;
using InventoryManagement.Api.Shared.Filters;
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
        public async Task<IActionResult> GetAll([FromQuery] ProductPageParams @params)
        {
            var pagedProducts = await _productService.GetPagedAsync(@params);
            return Ok(pagedProducts);
        }

        [HttpGet("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(Guid.Parse(id));
            return Ok(product);
        }

        [HttpGet("category/{categoryId}")]
        [ValidateGuid("categoryId")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var products = await _productService.GetByCategoryIdAsync(Guid.Parse(categoryId));
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDto dto)
        {
            var updated = await _productService.UpdateAsync(Guid.Parse(id), dto);
            return Ok(updated);
        }

        [HttpPatch("{id}/adjust-stock")]
        [ValidateGuid("id")]
        public async Task<IActionResult> AdjustStock(string id, [FromBody] AdjustProductStockDto dto)
        {
            var updatedProduct = await _productService.AdjustStockManuallyAsync(Guid.Parse(id), dto);
            return Ok(updatedProduct);
        }

        [HttpPatch("{id}/update-price")]
        [ValidateGuid("id")]
        public async Task<IActionResult> UpdatePrice(string id, [FromBody] UpdateProductPriceDto dto)
        {
            var updatedProduct = await _productService.UpdatePriceAsync(Guid.Parse(id), dto);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _productService.DeleteAsync(Guid.Parse(id));
            return NoContent();
        }
    }
}