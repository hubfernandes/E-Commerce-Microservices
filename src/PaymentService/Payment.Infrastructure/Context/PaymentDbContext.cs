using Microsoft.EntityFrameworkCore;
using Shared;

namespace Payment.Infrastructure.Context
{
    public class PaymentDbContext : BaseDbContext
    {
        public DbSet<Domain.Entities.Payment> Payments { get; set; }
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.PaymentMethod).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
