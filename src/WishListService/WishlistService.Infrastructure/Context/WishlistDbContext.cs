using Microsoft.EntityFrameworkCore;
using Shared;
using WishlistService.Domain.Entities;

namespace WishlistService.Infrastructure.Context
{
    public class WishlistDbContext : BaseDbContext
    {
        public WishlistDbContext(DbContextOptions<WishlistDbContext> options) : base(options) { }

        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.UserId)
                      .IsRequired()
                      .HasMaxLength(128);

                entity.HasMany(w => w.Items)
                      .WithOne()
                      .HasForeignKey(wi => wi.WishlistId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasKey(wi => wi.Id);
                entity.Property(wi => wi.ProductId)
                      .IsRequired();

                entity.HasIndex(wi => new { wi.WishlistId, wi.ProductId })
                      .IsUnique();
            });
        }
    }
}

