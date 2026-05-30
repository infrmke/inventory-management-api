using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; init; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
