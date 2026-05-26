using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.DTOs
{
    public record CreateCategoryDto(
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        string Name,

        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        string Description
    );
}
