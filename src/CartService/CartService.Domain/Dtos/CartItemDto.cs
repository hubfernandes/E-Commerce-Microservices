
namespace CartService.Domain.Dtos
{
    public record CartItemDto(int ProductId, int Quantity, decimal UnitPrice);
}