using CartService.Domain.Dtos;
using CartService.Domain.Entities;
using CartService.Infrastructure.Context;
using CartService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using Shared.Bases;
using Shared.Repository;
using System.Text.Json;

namespace CartService.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly CartContext _context;
        private readonly HttpClient _authClient;
        private readonly HttpClient _productClient;

        public CartRepository(CartContext dbContext, IHttpClientFactory httpClientFactory) : base(dbContext)
        {
            _context = dbContext;

            _productClient = httpClientFactory.CreateClient("ProductService")
            ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _authClient = httpClientFactory.CreateClient("AuthService")
               ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<List<CartItemDto>> GetCartItemsByUserIdAsync(string userId)
        {
            var carts = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .ToListAsync();

            var allItems = carts.SelectMany(c => c.Items)
                .Select(item => new CartItemDto(item.ProductId, item.Quantity, item.UnitPrice))
                .ToList();

            return allItems;
        }

        public override async Task<Cart> AddAsync(Cart cart)
        {
            await ValidateCustomerAsync(cart.UserId);
            await ValidateAndUpdateProductsAsync(cart);
            var result = await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async override Task<List<Cart>> GetAllAsync()
        {
            return await _context.Carts.Include(i => i.Items).ToListAsync();
        }
        public async override Task<Cart> GetByIdAsync(int id)
        {
            return await _context.Carts.AsNoTracking().Include(i => i.Items).FirstOrDefaultAsync(c => c.Id == id);
        }
        public override async Task<Cart> UpdateAsync(Cart cart)
        {
            await ValidateAndUpdateProductsAsync(cart);
            var existingCart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (existingCart == null)
                throw new KeyNotFoundException($"Cart with ID {cart.Id} not found");


            _context.Entry(existingCart).CurrentValues.SetValues(cart);
            existingCart.Items.Clear();
            existingCart.Items.AddRange(cart.Items);
            await _context.SaveChangesAsync();
            return existingCart;
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

        private async Task ValidateAndUpdateProductsAsync(Cart cart)
        {
            var productIds = cart.Items.Select(oi => oi.ProductId).Distinct().ToList();
            if (!productIds.Any()) return;

            var products = await FetchProductsAsync();
            ValidateProductExistence(productIds, products);
            UpdateCartItems(cart, products);
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

        private void ValidateProductExistence(List<int> productIds, Response<List<Product>> products)
        {
            var fetchedProductIds = products!.Data!.Select(p => p.Id).ToHashSet();
            var missingProductIds = productIds.Except(fetchedProductIds).ToList();

            if (missingProductIds.Any())
            {
                throw new InvalidOperationException($"Cannot create or update Cart. Products not found: {string.Join(",", missingProductIds)}");
            }
        }
        private void UpdateCartItems(Cart cart, Response<List<Product>> products)
        {
            foreach (var item in cart.Items)
            {
                var product = products!.Data!.First(p => p.Id == item.ProductId);
                if (item.UnitPrice != product.Price)
                {
                    item.UnitPrice = product.Price;
                    cart.CalculateTotal();
                }
            }
        }

        public async Task DeleteByUserIdAsync(string userId)
        {
            var cart = await _context.Carts.Where(i => i.UserId == userId).ToListAsync();
            if (cart.Any())
            {
                _context.Carts.RemoveRange(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
