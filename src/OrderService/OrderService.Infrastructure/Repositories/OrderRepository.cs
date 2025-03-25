using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using OrderService.Infrastructure.Interfaces;
using ProductService.Domain.Entities;
using Shared.Bases;
using Shared.Repository;
using System.Text.Json;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Domain.Entities.Order>, IOrderRepository
    {
        private readonly OrderDbContext _context;
        private readonly HttpClient _productClient;
        private readonly HttpClient _authClient;

        public OrderRepository(OrderDbContext dbContext, IHttpClientFactory httpClientFactory) : base(dbContext)
        {
            _context = dbContext;

            _productClient = httpClientFactory.CreateClient("ProductService")
              ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _authClient = httpClientFactory.CreateClient("AuthService")
                ?? throw new ArgumentNullException(nameof(httpClientFactory));

        }

        public override async Task<Domain.Entities.Order> AddAsync(Domain.Entities.Order order)
        {
            await ValidateCustomerAsync(order.CustomerId!);
            await ValidateAndUpdateProductsAsync(order);
            var result = await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        private async Task ValidateAndUpdateProductsAsync(Domain.Entities.Order order)
        {
            var productIds = order.Items.Select(oi => oi.ProductId).Distinct().ToList();
            if (productIds.Count == 0) return;

            var products = await FetchProductsAsync();
            ValidateProductExistence(productIds, products);
            UpdateOrderItems(order, products);
        }
        private async Task ValidateCustomerAsync(string customerId)
        {
            try
            {
                var response = await _authClient.GetAsync($"api/Account/{customerId}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Customer {customerId} not found in AuthService.");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to validate customer with AuthService: {ex.Message}", ex);
            }
        }
        private void ValidateProductExistence(List<int> productIds, Response<List<Product>> products)
        {
            var fetchedProductIds = products!.Data!.Select(p => p.Id).ToHashSet();
            var missingProductIds = productIds.Except(fetchedProductIds).ToList();

            if (missingProductIds.Any())
            {
                throw new InvalidOperationException($"Cannot create order. Products not found: {string.Join(",", missingProductIds)}");
            }
        }
        private async Task<Response<List<Product>>> FetchProductsAsync()
        {
            try
            {
                var response = await _productClient.GetAsync("api/products");
                response.EnsureSuccessStatusCode();

                var productJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine("productJson " + productJson);

                return JsonSerializer.Deserialize<Response<List<Product>>>(productJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Failed to deserialize products.");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to fetch products from ProductService: {ex.Message}", ex);
            }
        }
        private void UpdateOrderItems(Domain.Entities.Order order, Response<List<Product>> products)
        {
            foreach (var item in order.Items)
            {
                var product = products!.Data!.First(p => p.Id == item.ProductId);
                if (item.UnitPrice != product.Price)
                {
                    item.UnitPrice = product.Price;
                    order.CalculateTotalAmount();
                }
            }
        }
        public override async Task<List<Domain.Entities.Order>> GetAllAsync()
        {
            return await _context.Orders.Include(i => i.Items).ToListAsync();
        }
    }
}
