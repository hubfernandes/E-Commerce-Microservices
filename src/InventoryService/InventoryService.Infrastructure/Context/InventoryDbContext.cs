using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace InventoryService.Infrastructure.Context
{
    public class InventoryDbContext : BaseDbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InventoryItem>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => i.ProductId)
                .IsUnique();
        }

    }

}

