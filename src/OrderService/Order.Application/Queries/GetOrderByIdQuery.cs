using MediatR;
using Order.Domain.Dtos;
using Shared.Bases;

namespace Order.Application.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<Response<OrderDto>>;
}
