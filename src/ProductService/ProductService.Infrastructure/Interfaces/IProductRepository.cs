using InventoryService.Domain.Dtos;
using ProductService.Domain.Dtos;
using ProductService.Domain.Entities;
using Shared.Interfaces;

namespace ProductService.Infrastructure.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<ProductStockDto>> GetLowStockProductsAsync(List<int> productIds, List<LowStockDto> lowStockItems);
    }
}
