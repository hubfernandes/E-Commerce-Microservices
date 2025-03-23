using Order.Domain.Entities;

namespace Order.Application.Events
{
    public record OrderCanceledEvent(List<OrderItem> Items);
}
