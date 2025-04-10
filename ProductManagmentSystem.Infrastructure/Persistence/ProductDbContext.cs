using Microsoft.EntityFrameworkCore;
using ProductManagmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagmentSystem.Infrastructure.Persistence
{
    public class ProductDbContext: DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ImageUrl).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired(false); ;
                entity.Property(e => e.StockQuantity).IsRequired(false);
            });
        }
    }
}
