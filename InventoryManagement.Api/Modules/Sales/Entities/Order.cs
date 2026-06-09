using InventoryManagement.Api.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.Entities
{
    public class Order : IAuditableEntity
    {
        [Key]
        public Guid Id { get; init; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // relacionamento 1:N (1 pedido pode ter N produtos)
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
