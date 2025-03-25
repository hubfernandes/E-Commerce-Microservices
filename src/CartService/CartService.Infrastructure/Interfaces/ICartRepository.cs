using CartService.Domain.Dtos;
using CartService.Domain.Entities;
using Shared.Interfaces;

namespace CartService.Infrastructure.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<List<CartItemDto>> GetCartItemsByUserIdAsync(string userId);
        Task DeleteByUserIdAsync(string userId);
        Task<Cart?> GetByUserIdAsync(string userId);
    }
}
