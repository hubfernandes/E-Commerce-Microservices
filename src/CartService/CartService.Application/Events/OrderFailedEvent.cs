using CartService.Domain.Dtos;

namespace CartService.Application.Events
{
    public class OrderFailedEvent
    {
        public int OrderId { get; set; }
        public List<CartItemDto>? Items { get; set; }
    }
}
