using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Context;
using InventoryService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.RedisCache;
using Shared.Repository;

namespace InventoryService.Infrastructure.Repositories
{
    public class InventoryRepository : GenericRepository<InventoryItem>, IInventoryRepository
    {
        private readonly InventoryDbContext _context;
        private readonly ICacheService _cacheService;
        public InventoryRepository(InventoryDbContext dbContext, ICacheService cacheService) : base(dbContext)
        {
            _context = dbContext;
            _cacheService = cacheService;
        }

        public async Task<InventoryItem> GetByProductIdAsync(int productId)
        {
            string cacheKey = $"inventory:product:{productId}";
            var cachedItem = await _cacheService.GetAsync<InventoryItem>(cacheKey);

            if (cachedItem != null)
            {
                return cachedItem;
            }

            var inventoryItem = await _context.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventoryItem != null)
            {
                await _cacheService.SetAsync(cacheKey, inventoryItem, TimeSpan.FromMinutes(15));
            }

            return inventoryItem!;
        }

        public async Task<List<InventoryItem>> GetLowStockAsync(int threshold)
        {
            string cacheKey = $"inventory:lowstock:threshold:{threshold}";
            var cachedItems = await _cacheService.GetAsync<List<InventoryItem>>(cacheKey);

            if (cachedItems != null)
            {
                return cachedItems!;
            }

            var lowStockItems = await _context.InventoryItems
                .AsNoTracking()
                .Where(i => i.QuantityAvailable < threshold)
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, lowStockItems, TimeSpan.FromMinutes(10));
            return lowStockItems;
        }

        public override async Task<InventoryItem> AddAsync(InventoryItem entity)
        {
            var result = await _context.InventoryItems.AddAsync(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<InventoryItem>($"inventory:product:{entity.ProductId}", null!);
            return result.Entity;
        }

        public override async Task<InventoryItem> UpdateAsync(InventoryItem entity)
        {
            var existingItem = await _context.InventoryItems.FindAsync(entity.Id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {entity.Id} not found");
            }

            _context.Entry(existingItem).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<InventoryItem>($"inventory:product:{entity.ProductId}", null!);
            await _cacheService.SetAsync<List<InventoryItem>>($"inventory:lowstock:threshold:*", null!);

            return existingItem;
        }

        public override async Task DeleteAsync(InventoryItem entity)
        {
            _context.InventoryItems.Remove(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<InventoryItem>($"inventory:product:{entity.ProductId}", null!);
            await _cacheService.SetAsync<List<InventoryItem>>($"inventory:lowstock:threshold:*", null!);
        }

        public override async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
            {
                return false;
            }

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<InventoryItem>($"inventory:product:{item.ProductId}", null!);
            await _cacheService.SetAsync<List<InventoryItem>>($"inventory:lowstock:threshold:*", null!);

            return true;
        }


    }
}
