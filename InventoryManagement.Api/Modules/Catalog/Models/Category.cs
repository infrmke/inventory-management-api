using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; init; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
