using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; init; }

        [Required]
        public Guid OrderId { get; init; }

        [Required]
        public Guid ProductId { get; init; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public virtual Order? Order { get; set; }
    }
}
