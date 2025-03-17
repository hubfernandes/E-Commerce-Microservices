using Order.Infrastructure.Context;
using Order.Infrastructure.Interfaces;
using Shared.Repository;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order.Domain.Entities.Order>, IOrderRepository
    {
        public OrderRepository(OrderDbContext dbContext) : base(dbContext)
        {
        }

    }
}
