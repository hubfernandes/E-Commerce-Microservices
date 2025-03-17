using MediatR;
using Order.Domain.Dtos;
using Shared.Bases;

namespace Order.Application.Commands
{
    public record UpdateOrderCommand(int Id, string CustomerId, DateTime OrderDate, decimal TotalAmount, string Status, List<OrderItemDto> Items)
        : OrderDto(Id, CustomerId, OrderDate, TotalAmount, Status, Items), IRequest<Response<string>>;
}
