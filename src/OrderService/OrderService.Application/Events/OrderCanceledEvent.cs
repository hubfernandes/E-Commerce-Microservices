namespace OrderService.Application.Events
{
    public record OrderCanceledEvent(int ProductId, int Quantity);
}
