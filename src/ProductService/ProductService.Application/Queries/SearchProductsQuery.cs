using MediatR;
using ProductService.Domain.Dtos;
using Shared.Bases;

namespace ProductService.Application.Queries
{
    public record SearchProductsQuery(string Query) : IRequest<Response<List<ProductDto>>>;
}
