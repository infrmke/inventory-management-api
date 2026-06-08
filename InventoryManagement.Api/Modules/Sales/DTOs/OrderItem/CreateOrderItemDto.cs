using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.DTOs.OrderItem
{
    public record CreateOrderItemDto(
        [Required(ErrorMessage = "Product ID is required")]
        Guid ProductId,

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        int Quantity
    );
}
