using MediatR;
using OrderService.Domain.Dtos;
using Shared.Bases;

namespace OrderService.Application.Commands
{
    public record CreateOrderCommand(DateTime OrderDate, decimal TotalAmount, string Status, List<OrderItemDto> Items)
        : OrderDto(0, OrderDate, TotalAmount, Status, Items), IRequest<Response<string>>;
}
