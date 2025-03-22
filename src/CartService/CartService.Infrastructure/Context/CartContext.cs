using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace CartService.Infrastructure.Context
{
    public class CartContext : BaseDbContext
    {
        public CartContext(DbContextOptions<CartContext> options) : base(options)
        {
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.UserId).IsRequired();
                entity.HasMany(c => c.Items)
                      .WithOne()
                      .HasForeignKey("CartId");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => new { ci.CartId, ci.ProductId });
                entity.Property(ci => ci.Quantity).IsRequired();
                entity.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            });
        }

    }
}
