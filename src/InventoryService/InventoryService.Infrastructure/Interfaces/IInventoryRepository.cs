﻿using InventoryService.Domain.Entities;
using Shared.Interfaces;

namespace InventoryService.Infrastructure.Interfaces
{
    public interface IInventoryRepository : IGenericRepository<InventoryItem>
    {
        Task<InventoryItem> GetByProductIdAsync(int productId);
        Task<List<InventoryItem>> GetLowStockAsync(int threshold);
    }
}
