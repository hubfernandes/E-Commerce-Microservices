using CartService.Domain.Dtos;
using CartService.Domain.Entities;
using CartService.Infrastructure.Context;
using CartService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using Shared.Bases;
using Shared.RedisCache;
using Shared.Repository;
using System.Text.Json;

namespace CartService.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly CartContext _context;
        private readonly HttpClient _authClient;
        private readonly HttpClient _productClient;
        private readonly ICacheService _cacheService;

        public CartRepository(CartContext dbContext, IHttpClientFactory httpClientFactory, ICacheService cacheService) : base(dbContext)
        {
            _context = dbContext;
            _cacheService = cacheService;

            _productClient = httpClientFactory.CreateClient("ProductService")
            ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _authClient = httpClientFactory.CreateClient("AuthService")
               ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<List<CartItemDto>> GetCartItemsByUserIdAsync(string userId)
        {
            string cacheKey = $"cart_items_{userId}";
            var cachedItems = await _cacheService.GetAsync<List<CartItemDto>>(cacheKey);

            if (cachedItems != null)
            {
                return cachedItems;
            }

            var carts = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                .ToListAsync();

            var allItems = carts.SelectMany(c => c.Items)
                .Select(item => new CartItemDto(item.ProductId, item.Quantity, item.UnitPrice))
                .ToList();

            await _cacheService.SetAsync(cacheKey, allItems, TimeSpan.FromMinutes(30));
            return allItems;
        }

        public override async Task<Cart> AddAsync(Cart cart)
        {
            await ValidateCustomerAsync(cart.UserId);
            await ValidateAndUpdateProductsAsync(cart);
            var result = await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<List<CartItemDto>>($"cart_items_{cart.UserId}", null!);
            await _cacheService.SetAsync<Cart>($"cart_{cart.UserId}", null!);

            return result.Entity;
        }

        public async override Task<List<Cart>> GetAllAsync()
        {
            string cacheKey = "all_carts";
            var cachedCarts = await _cacheService.GetAsync<List<Cart>>(cacheKey);

            if (cachedCarts != null)
            {
                return cachedCarts;
            }

            var carts = await _context.Carts.Include(i => i.Items).ToListAsync();
            await _cacheService.SetAsync(cacheKey, carts, TimeSpan.FromHours(1));
            return carts;
        }
        public async override Task<Cart> GetByIdAsync(int id)
        {
            string cacheKey = $"cart_id_{id}";
            var cachedCart = await _cacheService.GetAsync<Cart>(cacheKey);

            if (cachedCart != null)
            {
                return cachedCart;
            }

            var cart = await _context.Carts
                .AsNoTracking()
                .Include(i => i.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart != null)
            {
                await _cacheService.SetAsync(cacheKey, cart, TimeSpan.FromMinutes(30));
            }
            return cart!;
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

            await _cacheService.SetAsync<Cart>($"cart_id_{cart.Id}", null!);
            await _cacheService.SetAsync<List<CartItemDto>>($"cart_items_{cart.UserId}", null!);
            await _cacheService.SetAsync<Cart>($"cart_{cart.UserId}", null!);

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
            string cacheKey = "all_products";
            var cachedProducts = await _cacheService.GetAsync<Response<List<Product>>>(cacheKey);

            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            try
            {
                var response = await _productClient.GetAsync("api/products");
                response.EnsureSuccessStatusCode();

                var productJson = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<Response<List<Product>>>(productJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Failed to deserialize products.");

                await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(15));
                return products;
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

                await _cacheService.SetAsync<List<CartItemDto>>($"cart_items_{userId}", null!);
                await _cacheService.SetAsync<Cart>($"cart_{userId}", null!);
            }
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            string cacheKey = $"cart_{userId}";
            var cachedCart = await _cacheService.GetAsync<Cart>(cacheKey);

            if (cachedCart != null)
            {
                return cachedCart;
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                await _cacheService.SetAsync(cacheKey, cart, TimeSpan.FromMinutes(30));
            }
            return cart;
        }
    }
}
