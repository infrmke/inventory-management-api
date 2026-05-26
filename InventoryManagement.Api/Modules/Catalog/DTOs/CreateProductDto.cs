using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.DTOs
{
    public record CreateProductDto(
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        string Name,

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        string Description,

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        decimal Price,

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be lower than zero")]
        int StockQuantity,

        [Required(ErrorMessage = "Category ID is required")]
        Guid CategoryId
    );
}
