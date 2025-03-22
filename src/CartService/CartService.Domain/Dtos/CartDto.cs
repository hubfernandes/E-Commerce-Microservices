namespace CartService.Domain.Dtos
{
    public record CartDto(int Id, string UserId, List<CartItemDto> Items);
}
