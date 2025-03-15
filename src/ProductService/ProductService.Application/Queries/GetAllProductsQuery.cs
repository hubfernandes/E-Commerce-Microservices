using MediatR;
using ProductService.Domain.Dtos;
using Shared.Bases;

namespace ProductService.Application.Queries
{
    public record GetAllProductsQuery : IRequest<Response<List<ProductDto>>>;
}