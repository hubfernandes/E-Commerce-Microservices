using InventoryService.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Dtos;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Context;
using ProductService.Infrastructure.Interfaces;
using Shared.RedisCache;
using Shared.Repository;
using System.Linq.Expressions;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ProductContext _context;
        private readonly ICacheService _cacheService;
        public ProductRepository(ProductContext dbContext, ICacheService cacheServic) : base(dbContext)
        {
            _cacheService = cacheServic;
            _context = dbContext;
        }

        public override async Task<List<Product>> GetAllAsync()
        {
            string cacheKey = "products:all";
            var cachedProducts = await _cacheService.GetAsync<List<Product>>(cacheKey);

            if (cachedProducts != null!)
            {
                return cachedProducts;
            }

            var products = await _context.Products.ToListAsync();
            await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromHours(1));
            return products;
        }
        public override async Task<Product> GetByIdAsync(int id)
        {
            string cacheKey = $"product:{id}";
            var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);

            if (cachedProduct != null!)
            {
                return cachedProduct;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null!)
            {
                await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
            }
            return product!;
        }
        public override async Task<List<Product>> GetByAsync(Expression<Func<Product, bool>> expression)
        {
            string cacheKey = $"products:filter:{expression.ToString().GetHashCode()}";
            var cachedProducts = await _cacheService.GetAsync<List<Product>>(cacheKey);

            if (cachedProducts != null!)
            {
                return cachedProducts;
            }

            var products = await _context.Products.AsNoTracking().Where(expression).ToListAsync();
            await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(15));
            return products;
        }

        public override async Task<Product> AddAsync(Product entity)
        {
            var result = await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<List<Product>>("products:all", null!);
            return result.Entity;
        }

        public override async Task<Product> UpdateAsync(Product entity)
        {
            var existingProduct = await _context.Products.FindAsync(entity.Id);
            if (existingProduct == null!)
            {
                throw new KeyNotFoundException($"Product with ID {entity.Id} not found");
            }

            _context.Entry(existingProduct).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<Product>($"product:{entity.Id}", null!);
            await _cacheService.SetAsync<List<Product>>("products:all", null!);

            return existingProduct;
        }

        public override async Task DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<Product>($"product:{entity.Id}", null!);
            await _cacheService.SetAsync<List<Product>>("products:all", null!);
        }

        public override async Task<bool> DeleteByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null!)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<Product>($"product:{id}", null!);
            await _cacheService.SetAsync<List<Product>>("products:all", null!);

            return true;
        }

        public override async Task<Product> GetByNameAsync(string name)
        {
            string cacheKey = $"product:name:{name.ToLowerInvariant()}";
            var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);

            if (cachedProduct != null!)
            {
                return cachedProduct;
            }

            var product = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
            if (product != null!)
            {
                await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
            }
            return product!;
        }

        public async Task<List<ProductStockDto>> GetLowStockProductsAsync(List<int> productIds, List<LowStockDto> lowStockItems)
        {
            string cacheKey = $"products:lowstock:{string.Join(",", productIds.OrderBy(id => id))}";
            var cachedProductStockDtos = await _cacheService.GetAsync<List<ProductStockDto>>(cacheKey);

            if (cachedProductStockDtos != null)
            {
                return cachedProductStockDtos;
            }

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var productStockDtos = products.Select(p =>
            {
                var stock = lowStockItems.First(i => i.ProductId == p.Id);
                return new ProductStockDto(p.Id, p.Name, stock.QuantityAvailable);
            }).ToList();

            await _cacheService.SetAsync(cacheKey, productStockDtos, TimeSpan.FromMinutes(10));
            return productStockDtos;
        }

    }
}
