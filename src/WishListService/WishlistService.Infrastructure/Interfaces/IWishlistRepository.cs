using Shared.Interfaces;
using WishlistService.Domain.Entities;

namespace WishlistService.Infrastructure.Interfaces
{
    public interface IWishlistRepository : IGenericRepository<Wishlist>
    {
        Task<Wishlist?> GetByUserIdAsync(string userId);
        Task DeleteByUserIdAsync(string userId);
        Task AddItemAsync(string userId, int productId);
        Task RemoveItemAsync(string userId, int productId);
    }
}
