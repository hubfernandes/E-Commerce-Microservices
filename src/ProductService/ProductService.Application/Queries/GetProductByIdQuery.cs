using MediatR;
using ProductService.Domain.Dtos;

namespace ProductService.Application.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
}