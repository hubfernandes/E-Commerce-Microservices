using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using Shared;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContext : BaseDbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain.Entities.Order>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Domain.Entities.Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.Id);

        }
    }
}
