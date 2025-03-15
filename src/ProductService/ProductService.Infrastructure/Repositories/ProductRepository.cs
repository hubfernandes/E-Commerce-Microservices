using ProductService.Domain.Entities;
using ProductService.Infrastructure.Interfaces;
using Shared.Repository;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ProductContext dbContext) : base(dbContext)
        {
        }


    }
}
