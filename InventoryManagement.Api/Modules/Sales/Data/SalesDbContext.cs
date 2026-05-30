using InventoryManagement.Api.Modules.Sales.Models;
using InventoryManagement.Api.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace InventoryManagement.Api.Modules.Sales.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                // configura o Guid Sequencial para a PK
                entity.Property(o => o.Id)
                      .HasDefaultValueSql("NEWSEQUENTIALID()");

                // garantindo a precisão correta para o total do pedido
                entity.Property(o => o.TotalPrice)
                    .HasColumnType("decimal(12,2)");

                // converte o Enum para String ao salvar (o SQL Server não possui um tipo de dado ENUM nativo)
                entity.Property(o => o.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30); // limite para o texto no banco
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");

                entity.Property(i => i.Id)
                      .HasDefaultValueSql("NEWSEQUENTIALID()");

                entity.Property(i => i.UnitPrice)
                    .HasColumnType("decimal(12,2)");

                // configuração do relacionamento 1:N e comportamento de exclusão
                entity.HasOne(i => i.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // override para aplicar os campos "CreatedAt" e "UpdatedAt"
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (IAuditableEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
