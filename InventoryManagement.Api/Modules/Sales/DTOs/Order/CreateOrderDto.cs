using InventoryManagement.Api.Modules.Sales.DTOs.OrderItem;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.DTOs.Order
{
    public record CreateOrderDto(
        [Required(ErrorMessage = "The order cannot be empty")]
        [MinLength(1, ErrorMessage = "The order must contain at least one item")]
        List<CreateOrderItemDto> Items
    );
}
