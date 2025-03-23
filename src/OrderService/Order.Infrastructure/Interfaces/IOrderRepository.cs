using Shared.Interfaces;

namespace Order.Infrastructure.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Domain.Entities.Order>
    {
    }
}
