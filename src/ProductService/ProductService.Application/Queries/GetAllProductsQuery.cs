using MediatR;
using ProductService.Domain.Dtos;

namespace ProductService.Application.Queries
{
    public record GetAllProductsQuery : IRequest<List<ProductDto>>;
}