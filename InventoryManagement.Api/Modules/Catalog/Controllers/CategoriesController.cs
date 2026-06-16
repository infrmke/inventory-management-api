using InventoryManagement.Api.Modules.Catalog.Dtos.Categories;
using InventoryManagement.Api.Modules.Catalog.Services.Categories;
using InventoryManagement.Api.Shared.Filters;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Modules.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CategoryPageParams @params)
        {
            var pagedCategories = await _categoryService.GetPagedAsync(@params);
            return Ok(pagedCategories);
        }

        [HttpGet("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> GetById(string id)
        {
            var category = await _categoryService.GetByIdAsync(Guid.Parse(id));
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryDto dto)
        {
            var updated = await _categoryService.UpdateAsync(Guid.Parse(id), dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [ValidateGuid("id")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _categoryService.DeleteAsync(Guid.Parse(id));
            return NoContent();
        }
    }
}