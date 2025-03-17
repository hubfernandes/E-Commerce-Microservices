namespace Order.Domain.Dtos
{
    public record OrderDto(int Id, string CustomerId, DateTime OrderDate, decimal TotalAmount, string Status, List<OrderItemDto> Items)
    {
        public OrderDto() : this(0, "", DateTime.MinValue, 0m, string.Empty, new List<OrderItemDto>()) { }
    }
}
