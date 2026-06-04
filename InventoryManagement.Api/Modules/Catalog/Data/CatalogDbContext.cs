using InventoryManagement.Api.Modules.Catalog.Models;
using InventoryManagement.Api.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Modules.Catalog.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                // definição explícita do nome da tabela
                entity.ToTable("Categories");

                // configura o Guid Sequencial para a PK
                entity.Property(c => c.Id)
                      .HasDefaultValueSql("NEWSEQUENTIALID()");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.Property(p => p.Id)
                      .HasDefaultValueSql("NEWSEQUENTIALID()");

                // e da precisão (price) da tabela (9.999.999.999,99)
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(12,2)");

                // configuração do relacionamento 1:N e comportamento de exclusão
                entity.HasOne<Category>()
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // não deleta a categoria se houver produtos nela
            });
        }

        // override para aplicar os campos "CreatedAt" e "UpdatedAt"
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            var currentTime = DateTime.UtcNow;

            foreach (var entityEntry in entries)
            {
                var entity = (IAuditableEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    entity.UpdatedAt = currentTime;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = currentTime;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
