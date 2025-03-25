using OrderService.Domain.Entities;
using Shared.Interfaces;
namespace OrderService.Infrastructure.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }
}
