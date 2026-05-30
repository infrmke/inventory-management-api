using InventoryManagement.Api.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Catalog.Models
{
    public class Category : IAuditableEntity
    {
        [Key]
        public Guid Id { get; init; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}
