using InventoryManagement.Api.Modules.Sales.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Sales.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options) { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                // garantindo a precisão correta para o total do pedido
                entity.Property(o => o.TotalPrice)
                    .HasColumnType("decimal(12,2)");

                // valor padrão para a data do pedido caso o C# não envie
                entity.Property(o => o.OrderDate)
                    .HasDefaultValueSql("GETDATE()");

                // converte o Enum para String ao salvar (o SQL Server não possui um tipo de dado ENUM nativo)
                entity.Property(o => o.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30); // limite para o texto no banco
            });
        }
}
}
