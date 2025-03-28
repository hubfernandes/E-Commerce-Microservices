using MediatR;
using Shared.Bases;

namespace ProductService.Application.Queries
{
    public record GetProductStockQuery(int ProductId) : IRequest<Response<int>>;
}
