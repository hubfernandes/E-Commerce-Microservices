using MediatR;
using Order.Domain.Dtos;
using Shared.Bases;

namespace Order.Application.Queries
{
    public record GetAllOrdersQuery : IRequest<Response<List<OrderDto>>>;
}
