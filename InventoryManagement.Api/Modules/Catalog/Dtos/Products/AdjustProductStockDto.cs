using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.Dtos.Products
{
    public record AdjustProductStockDto(
        [Required(ErrorMessage = "Quantity is required")]
        int Quantity
    );
}
