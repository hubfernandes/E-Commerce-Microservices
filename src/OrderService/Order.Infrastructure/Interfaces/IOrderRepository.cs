using Shared.Interfaces;

namespace Order.Infrastructure.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order.Domain.Entities.Order>
    {
    }
}
