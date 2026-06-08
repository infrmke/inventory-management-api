using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.DTOs.Product
{
    public record UpdateProductPriceDto(
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        decimal NewPrice
        );
}
