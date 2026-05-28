using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.DTOs
{
    public record CreateOrderItemDto(
    [Required(ErrorMessage = "Order ID is required")]
    Guid OrderId,

    [Required(ErrorMessage = "Product ID is required")]
    Guid ProductId,

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be lower than zero")]
    int Quantity,

    [Required(ErrorMessage = "Unit price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
    decimal UnitPrice
    );
}
