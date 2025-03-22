using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Context;
using InventoryService.Infrastructure.Interfaces;
using Shared.Repository;

namespace InventoryService.Infrastructure.Repositories
{
    internal class InventoryRepository : GenericRepository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(InventoryDbContext dbContext) : base(dbContext)
        {
        }

        public Task<InventoryItem> GetByProductIdAsync(string productId)
        {
            dbContext.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public Task<List<InventoryItem>> GetLowStockAsync(int threshold)
        {
            throw new NotImplementedException();
        }
    }
}
