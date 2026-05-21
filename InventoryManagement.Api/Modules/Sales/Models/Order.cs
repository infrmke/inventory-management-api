using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Modules.Sales.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // relacionamento 1:N (1 pedido pode ter N produtos)
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
