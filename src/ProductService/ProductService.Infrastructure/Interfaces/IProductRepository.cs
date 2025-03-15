using ProductService.Domain.Entities;
using Shared.Interfaces;

namespace ProductService.Infrastructure.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
    }
}
