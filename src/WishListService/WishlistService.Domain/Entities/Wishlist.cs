namespace WishlistService.Domain.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public List<WishlistItem>? Items { get; set; } = new();
    }
}
