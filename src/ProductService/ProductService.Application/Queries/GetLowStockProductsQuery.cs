using MediatR;
using ProductService.Domain.Dtos;
using Shared.Bases;

namespace ProductService.Application.Queries
{
    public record GetLowStockProductsQuery(int Threshold) : IRequest<Response<List<ProductStockDto>>>;
}
