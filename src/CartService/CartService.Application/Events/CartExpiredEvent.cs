using CartService.Domain.Dtos;

namespace CartService.Application.Events
{
    public class CartExpiredEvent
    {
        public int CartId { get; set; }
        public List<CartItemDto>? Items { get; set; }
    }
}
