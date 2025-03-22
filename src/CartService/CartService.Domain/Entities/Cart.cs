namespace CartService.Domain.Entities
{
    public class Cart
    {
        public Cart(int id, string userId, List<CartItem> items)
        {
            Id = id;
            UserId = userId;
            Items = items;
        }
        public Cart() { }
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal CalculateTotal() => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
