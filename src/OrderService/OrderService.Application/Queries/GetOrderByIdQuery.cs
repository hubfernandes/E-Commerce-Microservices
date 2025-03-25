using MediatR;
using OrderService.Domain.Dtos;
using Shared.Bases;

namespace OrderService.Application.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<Response<OrderDto>>;
}
