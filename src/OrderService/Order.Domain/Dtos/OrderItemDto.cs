namespace Order.Domain.Dtos
{
    public record OrderItemDto(int ProductionId, int Quantity, decimal UnitPrice);
}
