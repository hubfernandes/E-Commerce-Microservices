using MediatR;
using OrderService.Domain.Dtos;
using Shared.Bases;

namespace OrderService.Application.Queries
{
    public record GetAllOrdersQuery : IRequest<Response<List<OrderDto>>>;
}
