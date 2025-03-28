using Microsoft.EntityFrameworkCore;
using Shared.RedisCache;
using Shared.Repository;
using WishlistService.Domain.Entities;
using WishlistService.Infrastructure.Context;
using WishlistService.Infrastructure.Interfaces;

namespace WishlistService.Infrastructure.Repositories
{
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        private readonly WishlistDbContext _context;
        private readonly ICacheService _cacheService;

        public WishlistRepository(WishlistDbContext context, ICacheService cacheService) : base(context)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public override async Task<List<Wishlist>> GetAllAsync()
        {
            string cacheKey = "wishlists:all";
            var cachedWishlists = await _cacheService.GetAsync<List<Wishlist>>(cacheKey);
            if (cachedWishlists != null)
                return cachedWishlists;

            var wishlists = await _context.Wishlists.Include(w => w.Items).ToListAsync();
            await _cacheService.SetAsync(cacheKey, wishlists, TimeSpan.FromHours(1));
            return wishlists;
        }

        public override async Task<Wishlist> GetByIdAsync(int id)
        {
            string cacheKey = $"wishlist:{id}";
            var cachedWishlist = await _cacheService.GetAsync<Wishlist>(cacheKey);
            if (cachedWishlist != null)
                return cachedWishlist;

            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
            if (wishlist != null)
                await _cacheService.SetAsync(cacheKey, wishlist, TimeSpan.FromMinutes(30));
            return wishlist;
        }

        public async Task<Wishlist?> GetByUserIdAsync(string userId)
        {
            string cacheKey = $"wishlist:user:{userId}";
            var cachedWishlist = await _cacheService.GetAsync<Wishlist>(cacheKey);
            if (cachedWishlist != null)
                return cachedWishlist;

            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist != null)
                await _cacheService.SetAsync(cacheKey, wishlist, TimeSpan.FromMinutes(30));
            return wishlist;
        }

        public override async Task<Wishlist> AddAsync(Wishlist entity)
        {
            var result = await _context.Wishlists.AddAsync(entity);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<List<Wishlist>>("wishlists:all", null);
            await _cacheService.SetAsync<Wishlist>($"wishlist:user:{entity.UserId}", null);
            return result.Entity;
        }

        public override async Task<Wishlist> UpdateAsync(Wishlist entity)
        {
            var existingWishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.Id == entity.Id);
            if (existingWishlist == null)
                throw new KeyNotFoundException($"Wishlist with ID {entity.Id} not found");

            _context.Entry(existingWishlist).CurrentValues.SetValues(entity);
            existingWishlist.Items.Clear();
            existingWishlist.Items.AddRange(entity.Items);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<Wishlist>($"wishlist:{entity.Id}", null);
            await _cacheService.SetAsync<Wishlist>($"wishlist:user:{entity.UserId}", null);
            await _cacheService.SetAsync<List<Wishlist>>("wishlists:all", null);
            return existingWishlist;
        }

        public override async Task<bool> DeleteByIdAsync(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
                return false;

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            await _cacheService.SetAsync<Wishlist>($"wishlist:{id}", null);
            await _cacheService.SetAsync<Wishlist>($"wishlist:user:{wishlist.UserId}", null);
            await _cacheService.SetAsync<List<Wishlist>>("wishlists:all", null);
            return true;
        }

        public async Task DeleteByUserIdAsync(string userId)
        {
            var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();

                await _cacheService.SetAsync<Wishlist>($"wishlist:{wishlist.Id}", null);
                await _cacheService.SetAsync<Wishlist>($"wishlist:user:{userId}", null);
                await _cacheService.SetAsync<List<Wishlist>>("wishlists:all", null);
            }
        }

        public async Task AddItemAsync(string userId, int productId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                await _context.Wishlists.AddAsync(wishlist);
            }

            if (!wishlist.Items.Any(i => i.ProductId == productId))
            {
                wishlist.Items.Add(new WishlistItem { ProductId = productId });
                await _context.SaveChangesAsync();

                await _cacheService.SetAsync<Wishlist>($"wishlist:{wishlist.Id}", null);
                await _cacheService.SetAsync<Wishlist>($"wishlist:user:{userId}", null);
                await _cacheService.SetAsync<List<Wishlist>>("wishlists:all", null);
            }
        }

        public async Task RemoveItemAsync(string userId, int productId)
        {
            var wishlist = await _context.Wishlists.Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist != null)
            {
                var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    wishlist.Items.Remove(item);
                    await _context.SaveChangesAsync();

                    await _cacheService.SetAsync<Wishlist>($"wishlist:{wishlist.Id}", null);
                    await _cacheService.SetAsync<Wishlist>($"wishlist:user:{userId}", null);
                    await _cacheService.SetAsync<List<Wishlist>>("wishlists:all", null);
                }
            }
        }
    }
}
