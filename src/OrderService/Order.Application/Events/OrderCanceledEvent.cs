namespace Order.Application.Events
{
    public record OrderCanceledEvent(int ProductId, int Quantity);
}
