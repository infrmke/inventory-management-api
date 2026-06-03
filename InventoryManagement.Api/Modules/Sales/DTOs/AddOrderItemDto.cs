using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.DTOs
{
    public record AddOrderItemDto(
        [Required(ErrorMessage = "Product ID is required")]
        Guid ProductId,

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        int Quantity,

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
        decimal UnitPrice
    );
}
