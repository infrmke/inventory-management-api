using InventoryManagement.Api.Modules.Catalog.DTOs;
using InventoryManagement.Api.Modules.Catalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Modules.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
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

            if (product == null) return NotFound(new { error = "Product not found" });

            return Ok(product);
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

            if(updated == null) return NotFound(new { error = "Product not found" });

            return Ok(updated);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteAsync(id);

            if (!deleted) return NotFound(new { error = "Product not found" });

            return NoContent();
        }
    }
}
