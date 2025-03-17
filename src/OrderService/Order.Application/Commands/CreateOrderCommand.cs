using MediatR;
using Order.Domain.Dtos;
using Shared.Bases;

namespace Order.Application.Commands
{
    public record CreateOrderCommand(string CustomerId, DateTime OrderDate, decimal TotalAmount, string Status, List<OrderItemDto> Items)
        : OrderDto(0, CustomerId, OrderDate, TotalAmount, Status, Items), IRequest<Response<string>>;
}
