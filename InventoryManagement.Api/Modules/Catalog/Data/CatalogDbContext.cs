using InventoryManagement.Api.Modules.Catalog.Models;
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
            });

            modelBuilder.Entity<Product>(entity =>
            {
                // definição explícita do nome
                entity.ToTable("Products");

                // e da precisão (price) da tabela (9.999.999.999,99)
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(12,2)");
            });
        }
    }
}
