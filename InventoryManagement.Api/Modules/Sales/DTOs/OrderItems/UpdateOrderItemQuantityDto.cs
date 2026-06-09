using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.DTOs.OrderItems
{
    public record UpdateOrderItemQuantityDto(
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        int Quantity
    );
}
