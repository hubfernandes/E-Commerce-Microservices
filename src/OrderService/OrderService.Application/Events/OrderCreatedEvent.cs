namespace OrderService.Application.Events
{
    public record OrderCreatedEvent(int OrderId, string CustomerId, decimal TotalAmount);
}
