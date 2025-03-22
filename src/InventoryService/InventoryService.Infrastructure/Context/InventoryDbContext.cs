using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace InventoryService.Infrastructure.Context
{
    public class InventoryDbContext : BaseDbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
    }
}
