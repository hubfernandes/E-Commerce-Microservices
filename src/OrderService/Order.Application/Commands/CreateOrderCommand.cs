using MediatR;
using Order.Domain.Dtos;
using Shared.Bases;

namespace Order.Application.Commands
{
    public record CreateOrderCommand(DateTime OrderDate, decimal TotalAmount, string Status, List<OrderItemDto> Items)
        : OrderDto(0, OrderDate, TotalAmount, Status, Items), IRequest<Response<string>>;
}
