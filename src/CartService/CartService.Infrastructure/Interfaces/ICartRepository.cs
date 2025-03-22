using CartService.Domain.Entities;
using Shared.Interfaces;

namespace CartService.Infrastructure.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
    }
}
