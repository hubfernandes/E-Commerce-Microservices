namespace OrderService.Domain.Dtos
{
    public record OrderItemDto(int ProductId, int Quantity, decimal UnitPrice);
}
