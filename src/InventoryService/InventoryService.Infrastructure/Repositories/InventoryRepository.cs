using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Context;
using InventoryService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace InventoryService.Infrastructure.Repositories
{
    internal class InventoryRepository : GenericRepository<InventoryItem>, IInventoryRepository
    {
        private readonly InventoryDbContext _context;
        public InventoryRepository(InventoryDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<InventoryItem> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId) ?? throw new KeyNotFoundException();
        }

        public Task<List<InventoryItem>> GetLowStockAsync(int threshold)
        {
            return _context.InventoryItems.Where(i => i.QuantityAvailable < threshold).ToListAsync();
        }
    }
}
